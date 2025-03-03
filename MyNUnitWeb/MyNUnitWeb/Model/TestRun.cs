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
    public int Id { get; set; }

    public int TotalTests { get; set; }

    public int PassedTests { get; set; }

    public int FailedTests { get; set; }

    public int IgnoredTests { get; set; }

    public List<TestClassModel> TestClasses { get; set; }
}

/// <summary>
/// Represents a test class within a test run.
/// </summary>
public class TestClassModel
{
    public int Id { get; set; }

    public int TestRunModelId { get; set; }

    public TestRun TestRun { get; set; }

    public string Name { get; set; }

    public List<TestDetailModel> TestDetails { get; set; }
}

/// <summary>
/// Represents a single test case within a test class.
/// </summary>
public class TestDetailModel
{
    public int Id { get; set; }

    public int TestClassModelId { get; set; }

    public TestClassModel TestClass { get; set; }

    public string TestName { get; set; }

    public string Status { get; set; }

    public long ExecutionTime { get; set; }

    public string? ErrorMessage { get; set; }

    public string? IgnoreMessage { get; set; }
}