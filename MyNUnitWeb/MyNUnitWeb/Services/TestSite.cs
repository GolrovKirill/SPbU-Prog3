// <copyright file="TestSite.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitWeb.Services;

using System.Collections.Concurrent;
using MyNUnit;
using MyNUnitWeb.Data;
using MyNUnitWeb.Model;

/// <summary>
/// Provides services for running tests using MyNUnit.
/// </summary>
public class TestSite
{
    private readonly WebData dbContext;

    public TestSite(WebData dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Runs the tests in the directory associated with the given client ID.
    /// </summary>
    public async Task<TestRun?> RunTests(string clientId)
    {
        var clientUploadPath = Path.Combine(
            Path.GetTempPath(),
            "Upload",
            clientId);

        if (!Directory.Exists(clientUploadPath))
        {
            return null;
        }

        using var runner = new MyNUnit();
        var myNUnitResults = await runner.RunTests(clientUploadPath);

        var testRunModel = await ConvertMyNUnitToTestRunModel(myNUnitResults);

        if (testRunModel != null)
        {
            await this.NewMethod(testRunModel);
        }

        return testRunModel;
    }

    /// <summary>
    /// Converts MyNUnit test results into a <see cref="TestRun"/> for storage.
    /// </summary>
    private static async Task<TestRun?> ConvertMyNUnitToTestRunModel(List<TestResultClass>? testResultsMyNUnit)
    {
        if (testResultsMyNUnit == null || testResultsMyNUnit.Count == 0)
        {
            return new TestRun
            {
                TotalTests = 0,
                PassedTests = 0,
                FailedTests = 0,
                IgnoredTests = 0,
            };
        }

        var testClasses = new ConcurrentBag<TestClassModel>();
        var testRun = new TestRun();

        var tasks = testResultsMyNUnit.Select(testClass => Task.Run(() =>
        {
            var testClassModel = new TestClassModel
            {
                Id = 0,
                TestRunModelId = testRun.Id,
                Name = testClass.ClassName,
            };

            var testDetails = testClass.TestResults.Select((result, index) => new TestDetailModel
            {
                Id = index,
                TestClassModelId = testClassModel.Id,
                TestName = result.TestName,
                Status = result.IgnoreMessage != null ? "Ignored" : result.IsPassed ? "Passed" : "Failed",
                ExecutionTime = (long)result.Time.TotalMilliseconds,
                ErrorMessage = result.IsPassed ? null : result.ExceptionMessage,
                IgnoreMessage = result.IgnoreMessage,
            }).ToList();

            foreach (var detail in testDetails)
            {
                detail.TestClass = testClassModel;
            }

            testClassModel.TestDetails = testDetails;
            testClassModel.TestRun = testRun;

            testClasses.Add(testClassModel);
        }));

        await Task.WhenAll(tasks);

        testRun.TestClasses = [.. testClasses];
        testRun.TotalTests = testClasses.Sum(c => c.TestDetails?.Count ?? 0);
        testRun.PassedTests = testClasses.Sum(c => c.TestDetails?.Count(td => td.Status == "Passed") ?? 0);
        testRun.FailedTests = testClasses.Sum(c => c.TestDetails?.Count(td => td.Status == "Failed") ?? 0);
        testRun.IgnoredTests = testClasses.Sum(c => c.TestDetails?.Count(td => td.Status == "Ignored") ?? 0);

        return testRun;
    }

    private Task NewMethod(TestRun testRunModel)
    {
        return this.SaveTestResultsToDatabase(testRunModel);
    }

    /// <summary>
    /// Saves the test run results to the database.
    /// </summary>
    private async Task SaveTestResultsToDatabase(TestRun? testRunModel)
    {
        if (testRunModel != null)
        {
            this.dbContext.TestRuns.Add(testRunModel);
            await this.dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentNullException(nameof(testRunModel), "TestRun model cannot be null.");
        }
    }
}