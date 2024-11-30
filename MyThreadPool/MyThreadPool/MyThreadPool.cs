namespace MyThreadPool;

/// <summary>
/// Class that implements thread pool.
/// </summary>
public class MyThreadPool
{
    private readonly CancellationTokenSource clt = new();

    private readonly Queue<Action> queue = new();

    private readonly Thread[] threads;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="countThreads">Number of threads.</param>
    public MyThreadPool(int countThreads)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(countThreads);

        threads = new Thread[countThreads];
        for (var i = 0; i < countThreads; i++)
        {
            threads[i] = new Thread(Worker);
            threads[i].Start();
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
        return new MyTask<TResult>(func, clt.Token, queue);
    }

    /// <summary>
    /// The method that shutdown threads.
    /// </summary>
    public void Shutdown()
    {
        clt.Cancel();
        lock (queue)
        {
            Monitor.PulseAll(queue);
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private void Worker()
    {
        while (!clt.IsCancellationRequested || queue.Count > 0)
        {
            Action task;
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    if (clt.IsCancellationRequested)
                    {
                        return;
                    }

                    Monitor.Wait(queue);
                }

                task = queue.Dequeue();
            }

            task();
        }
    }
}