// <copyright file="TestResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class with information about run test.
/// </summary>
public record TestResult
{
    /// <summary>
    /// Gets or sets name test.
    /// </summary>
    public string? TestName { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether result test.
    /// </summary>
    public bool Passed { get; init; }

    /// <summary>
    /// Gets or sets the reason for the result.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Gets or sets time work test.
    /// </summary>
    public TimeSpan Duration { get; init; }
}