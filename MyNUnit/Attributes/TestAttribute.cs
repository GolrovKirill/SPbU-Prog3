namespace Attributes;

/// <summary>
/// Class that implements Test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; }
    public string? Ignore { get; }

    public TestAttribute(Type? expected = null, string? ignore = null)
    {
        Expected = expected;
        Ignore = ignore;
    }

    public TestAttribute(string ignore)
    {
        Expected = null;
        Ignore = ignore;
    }
}
