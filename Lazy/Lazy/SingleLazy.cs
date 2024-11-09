namespace Lazy;

/// <inheritdoc />
public class SingleLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private T? result;
    private volatile bool isCalculated;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Transmitted function.</param>
    public SingleLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    }

    /// <inheritdoc/>
    public T? Get()
    {
        if (!isCalculated)
        {
            isCalculated = true;
            try
            {
                result = supplier();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error while executing supplier function.", ex);
            }
        }

        return result;
    }
}
