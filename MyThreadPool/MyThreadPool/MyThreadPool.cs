// <copyright file="MyThreadPool.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyThreadPool;

/// <summary>
/// Class that implements thread pool.
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly Queue<Action> queue = new();

    private readonly Thread[] threads;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="countThreads">Number of threads.</param>
    public MyThreadPool(int countThreads)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(countThreads);

        this.threads = new Thread[countThreads];
        for (var i = 0; i < countThreads; i++)
        {
            this.threads[i] = new Thread(this.ExecuteTasks);
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Method to add a task to the thread pool.
    /// </summary>
    /// <typeparam name="TResult">Type result.</typeparam>
    /// <param name="func">Function to need calculate.</param>
    /// <returns>Task result.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        return new MyTask<TResult>(func, this.cancellationTokenSource.Token, this.queue);
    }

    /// <summary>
    /// The method that shutdown threads.
    /// </summary>
    public void Shutdown()
    {
        this.cancellationTokenSource.Cancel();
        lock (this.queue)
        {
            Monitor.PulseAll(this.queue);
        }

        foreach (var thread in this.threads)
        {
            if (thread.IsAlive)
            {
                thread.Join();
            }
        }
    }

    private void ExecuteTasks()
    {
        while (true)
        {
            Action task;
            lock (this.queue)
            {
                while (this.queue.Count == 0)
                {
                    if (this.cancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    Monitor.Wait(this.queue);
                }

                task = this.queue.Dequeue();
            }

            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            task();
        }
    }
}