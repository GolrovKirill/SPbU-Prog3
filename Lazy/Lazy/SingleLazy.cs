namespace Lazy;

/// <inheritdoc />
public class SingleLazy<T>(Func<T> supplier) : ILazy<T>
{
    private T? result;

    /// <summary>
    /// Checking the availability of the result.
    /// </summary>
    public bool FlagResult { get; private set; }

    /// <inheritdoc/>
    public T Get()
    {
        if (!FlagResult)
        {
            FlagResult = true;
            result = supplier();
        }

        ArgumentNullException.ThrowIfNull(supplier());

        return result;
    }
}
