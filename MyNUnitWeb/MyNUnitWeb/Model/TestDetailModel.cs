// <copyright file="TestDetailModel.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Model;

/// <summary>
/// Represents a single test case within a test class.
/// </summary>
public class TestDetailModel
{
    public TestDetailModel()
    {
    }

    public TestDetailModel(int id, int testClassModelId, string testName, string status, long executionTime, string? errorMessage, string? ignoreMessage)
    {
        this.Id = id;
        this.TestClassModelId = testClassModelId;
        this.TestName = testName;
        this.Status = status;
        this.ExecutionTime = executionTime;
        this.ErrorMessage = errorMessage;
        this.IgnoreMessage = ignoreMessage;
    }

    public int Id { get; set; }

    public int TestClassModelId { get; set; }

    public TestClassModel TestClass { get; set; } = null!;

    public string TestName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public long ExecutionTime { get; set; }

    public string? ErrorMessage { get; set; }

    public string? IgnoreMessage { get; set; }
}
