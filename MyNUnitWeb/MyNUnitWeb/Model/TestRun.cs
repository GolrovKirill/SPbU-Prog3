// <copyright file="TestRun.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Model;

/// <summary>
/// Represents the result of a test run.
/// </summary>
public class TestRun
{
    public TestRun()
    {
        this.TestClasses = [];
    }

    public int Id { get; set; }

    public int TotalTests { get; set; }

    public int PassedTests { get; set; }

    public int FailedTests { get; set; }

    public int IgnoredTests { get; set; }

    public List<TestClassModel> TestClasses { get; set; }
}
