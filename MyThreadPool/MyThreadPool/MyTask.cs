// <copyright file="MyTask.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyThreadPool;

/// <summary>
/// Class that implements the task interface.
/// </summary>
/// <typeparam name="TResult">The type of result of the task calculation.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> function;
    private readonly object lockObject = new();
    private readonly Queue<Action> queue;
    private readonly CancellationToken cancellationToken;
    private Exception? exception;
    private TResult? funcResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="func">Task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="queue">Task queue.</param>
    public MyTask(Func<TResult> func, CancellationToken clt, Queue<Action> queue)
    {
        this.function = func;
        this.cancellationToken = clt;
        this.queue = queue;
        lock (queue)
        {
            this.queue.Enqueue(this.Execute);
            Monitor.Pulse(queue);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the task has been completed successfully.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <inheritdoc/>
    public TResult? Result
    {
        get
        {
            lock (this.lockObject)
            {
                while (!this.IsCompleted && this.exception is null)
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.IsCompleted = true;
                        throw new TaskCanceledException();
                    }

                    Monitor.Wait(this.lockObject);
                }

                if (this.exception is not null)
                {
                    throw new AggregateException(this.exception);
                }

                return this.funcResult;
            }
        }
    }

    /// <inheritdoc/>
    IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
    {
#pragma warning disable CS8604 // ¬озможно, аргумент-ссылка, допускающий значение NULL.
        return new MyTask<TNewResult>(() => func(this.Result), this.cancellationToken, this.queue);
#pragma warning restore CS8604 // ¬озможно, аргумент-ссылка, допускающий значение NULL.
    }

    private void Execute()
    {
        lock (this.lockObject)
        {
            if (this.IsCompleted)
            {
                return;
            }

            if (this.cancellationToken.IsCancellationRequested)
            {
                this.IsCompleted = true;
                Monitor.PulseAll(this.lockObject);
                return;
            }

            try
            {
                this.funcResult = this.function();
                this.IsCompleted = true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            finally
            {
                Monitor.PulseAll(this.lockObject);
            }
        }
    }
}