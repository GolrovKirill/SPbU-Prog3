// <copyright file="TestAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Attributes;

/// <summary>
/// Class that implements Test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets expected test.
    /// </summary>
    public Type? Expected { get; }

    /// <summary>
    /// Gets ignore test.
    /// </summary>
    public string? Ignore { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="expected">Expected attribute.</param>
    /// <param name="ignore">Ignore attribute.</param>
    public TestAttribute(Type? expected = null, string? ignore = null)
    {
        Expected = expected;
        Ignore = ignore;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="ignore">Ignore attribute.</param>
    public TestAttribute(string ignore)
    {
        Expected = null;
        Ignore = ignore;
    }
}
