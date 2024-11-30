namespace MyThreadPool;

/// <summary>
/// Interface for tasks accepted for execution.
/// </summary>
/// <typeparam name="TResult">Type of result.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether returns true if the task is completed, otherwise false.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the completed task, it can also be null.
    /// </summary>
    TResult? Result { get; }

    /// <summary>
    /// Gets a new task and adds it to the queue.
    /// </summary>
    /// <param name="function">New function for task.</param>
    /// <typeparam name="TNewResult">New type of result.</typeparam>
    /// <returns>New task.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function);
}
