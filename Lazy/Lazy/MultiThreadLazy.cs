namespace Lazy;

/// <inheritdoc />
public class MultiThreadLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private T? result;
    private bool isCalculated;
    private readonly object lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Transmitted function.</param>
    public MultiThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    }

    /// <inheritdoc/>
    public T? Get()
    {
        if (isCalculated)
        {
            return result;
        }

        lock (lockObject)
        {
            if (!isCalculated)
            {
                try
                {
                    result = supplier();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error while executing supplier function.", ex);
                }
                finally
                {
                    isCalculated = true;
                }
            }
        }

        return result;
    }
}