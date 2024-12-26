// <copyright file="TestResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Class with information about run test.
/// </summary>
public record TestResult(string? TestName, bool Passed, string? Message, TimeSpan Duration)
{
}