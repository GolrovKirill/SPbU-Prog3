namespace Lazy;

/// <inheritdoc />
public class MultipleLazy<T>(Func<T> supplier) : ILazy<T>
{
    private T? result;

    /// <summary>
    /// Checking the availability of the result.
    /// </summary>
    public bool FlagResult { get; private set; }

    private readonly object lockObject = new();

    /// <inheritdoc/>
    public T Get()
    {
        if (!FlagResult)
        {
            lock (lockObject)
            {
                FlagResult = true;
                result = supplier();
            }
        }

        ArgumentNullException.ThrowIfNull(supplier());

        return result;
    }
}