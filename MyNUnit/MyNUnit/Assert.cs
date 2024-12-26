namespace MyNUnit;

/// <summary>
/// Class that checks the validity of the condition.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Method that throws exception if condition is false.
    /// </summary>
    /// <param name="condition">What we check for accuracy.</param>
    public static void That(bool condition)
    {
        if (!condition)
        {
            throw new AssertFailException();
        }
    }
}