// <copyright file="TestResult.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnit;

public record TestResult(string TestName, bool IsPassed, Type? ExceptionType, string? ExceptionMessage, string? IgnoreMessage, TimeSpan Time);