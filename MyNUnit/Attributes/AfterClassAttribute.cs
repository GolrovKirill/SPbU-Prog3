namespace Attributes;

/// <summary>
/// Class that implements after class of test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute { }