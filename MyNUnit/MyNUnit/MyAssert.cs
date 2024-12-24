namespace MyNUnit;

public static class MyAssert
{
    /// <summary>
    /// Checks if the specified condition is true.
    /// </summary>
    /// <param name="condition">The condition to be checked.</param>
    /// <exception cref="AssertionException">Throws if the assertion fails.</exception>
    public static void IsTrue(bool condition)
    {
        if (!condition)
        {
            throw new AssertionException("Assertion failed");
        }
    }

    /// <summary>
    /// Asserts that the specified function throws an exception of the expected type.
    /// </summary>
    /// <typeparam name="T">Type of the expected exception.</typeparam>
    /// <param name="function">Function to be checked.</param>
    /// <exception cref="AssertionException">Throws if the assertion fails.</exception>
    public static void Throws<T>(Func<T> function)
    {
        try
        {
            function();
        }
        catch (Exception ex)
        {
            if (typeof(T) != ex.GetType())
            {
                throw new AssertionException("Expected exception of type " + typeof(T).Name + " but got " + ex.GetType().Name);
            }
        }
    }
}

/// <summary>
/// Custom exception class for assertion failures.
/// </summary>
public class AssertionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionException"/> class.
    /// </summary>
    public AssertionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public AssertionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="innerException">Inner exception.</param>
    public AssertionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}