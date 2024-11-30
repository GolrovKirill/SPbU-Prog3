namespace MyThreadPool;

/// <summary>
/// Class that implements the task interface.
/// </summary>
/// <typeparam name="TResult">The type of result of the task calculation.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> function;
    private readonly object lockObject = new ();
    private readonly Queue<Action> queue;
    private readonly CancellationToken clt;
    private Exception? exception;
    private TResult? funcResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="func">Task.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="queue">Task queue.</param>
    public MyTask(Func<TResult> func, CancellationToken cancellationToken, Queue<Action> queue)
    {
        function = func;
        clt = cancellationToken;
        this.queue = queue;
        lock (queue)
        {
            this.queue.Enqueue(Execute);
            Monitor.Pulse(queue);
        }
    }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <inheritdoc/>
    public TResult? Result
    {
        get
        {
            lock (lockObject)
            {
                while (!IsCompleted && exception is null)
                {
                    if (clt.IsCancellationRequested)
                    {
                        throw new TaskCanceledException();
                    }

                    Monitor.Wait(lockObject);
                }

                if (exception is not null)
                {
                    throw new AggregateException(exception);
                }


                return funcResult;
            }
        }
    }

    /// <inheritdoc/>
    IMyTask<TNewResult> IMyTask<TResult>.ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
    {
        return new MyTask<TNewResult>(() => func(Result), clt, queue);
    }

    private void Execute()
    {
        lock (lockObject)
        {
            if (IsCompleted)
            {
                return;
            }

            if (clt.IsCancellationRequested)
            {
                Monitor.PulseAll(lockObject);
                return;
            }

            try
            {
                funcResult = function();
                IsCompleted = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                Monitor.PulseAll(lockObject);
            }
        }
    }
}