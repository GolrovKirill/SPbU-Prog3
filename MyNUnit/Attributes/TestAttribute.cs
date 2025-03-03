// <copyright file="TestAttribute.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace Attributes;

/// <summary>
/// Class that implements Test attribute.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TestAttribute"/> class.
/// </remarks>
/// <param name="expectedException">The type of the expected exception. Default is null.</param>
/// <param name="ignoreMessage">The message that will mark the test as ignored. Default is an empty string.</param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute(Type? expectedException = null, string? ignoreMessage = null) : Attribute
{ /// <summary>
  /// Gets the type of the expected exception that the test should throw.
  /// </summary>
    public Type? ExpectedException { get; } = expectedException;

    /// <summary>
    /// Gets the message that explains why the test is ignored.
    /// </summary>
    public string? IgnoreMessage { get; } = ignoreMessage;

    /// <summary>
    /// An indicator that the test is being ignored.
    /// </summary>
    public bool IsIgnore => !string.IsNullOrEmpty(IgnoreMessage);
}
