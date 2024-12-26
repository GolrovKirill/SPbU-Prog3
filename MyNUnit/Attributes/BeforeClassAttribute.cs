namespace Attributes;

/// <summary>
/// Class that implements before class of test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute { }