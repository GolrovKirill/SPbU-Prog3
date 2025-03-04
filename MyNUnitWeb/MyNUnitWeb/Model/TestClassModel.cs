// <copyright file="TestClassModel.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Model;

/// <summary>
/// Represents a test class within a test run.
/// </summary>
public class TestClassModel
{
    public TestClassModel()
    {
    }

    public TestClassModel(int id, int testRunModelId, string name)
    {
        this.Id = id;
        this.TestRunModelId = testRunModelId;
        this.Name = name;
    }

    public int Id { get; set; }

    public int TestRunModelId { get; set; }

    public TestRun TestRun { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<TestDetailModel>? TestDetails { get; set; }
}