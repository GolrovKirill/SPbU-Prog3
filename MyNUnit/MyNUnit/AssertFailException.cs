namespace MyNUnit;

/// <summary>
/// Class that implements AssertFailException.
/// </summary>
public class AssertFailException : Exception
{
    public AssertFailException()
    {
    }

    public AssertFailException(string message) : base(message)
    {
    }
}