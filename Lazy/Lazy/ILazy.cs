namespace Lazy;

public interface ILazy<T>
{
    /// <summary>
    /// Returns the result of the function passed to Lazy.
    /// </summary>
    /// <returns>Result function.</returns>
    T? Get();
}