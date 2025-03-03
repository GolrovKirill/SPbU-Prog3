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
public class TestSite(WebData dbContext)
{
    /// <summary>
    /// Runs the tests in the directory associated with the given client ID.
    /// </summary>
    public async Task<TestRun?> RunTests(string clientId)
    {
        var clientUploadPath = Path.Combine(Path.GetTempPath(), "Upload", clientId);

        if (!Directory.Exists(clientUploadPath))
        {
            return null;
        }

        using var runner = new MyNUnit();
        var myNUnitResults = await runner.RunTests(clientUploadPath);

        var testRunModel = await ConvertMyNUnitToTestRunModel(myNUnitResults);
        await SaveTestResultsToDatabase(testRunModel);
        return testRunModel;
    }

    /// <summary>
    /// Converts MyNUnit test results into a <see cref="TestRun"/> for storage.
    /// </summary>
    private static Task<TestRun> ConvertMyNUnitToTestRunModel(List<TestResultClass>? testResultsMyNUnit)
    {
        if (testResultsMyNUnit == null || testResultsMyNUnit.Count == 0)
        {
            return Task.FromResult(new TestRun
            {
                TotalTests = 0,
                PassedTests = 0,
                FailedTests = 0,
                IgnoredTests = 0,
                TestClasses = new List<TestClassModel>(),
            });
        }

        var testClasses = new ConcurrentBag<TestClassModel>();

        Parallel.ForEach(testResultsMyNUnit, testClass =>
        {
            var testDetails = testClass.TestResults.Select(result => new TestDetailModel
            {
                TestName = result.TestName,
                Status = result.IgnoreMessage != null ? "Ignored" : result.IsPassed ? "Passed" : "Failed",
                ExecutionTime = (long)result.Time.TotalMilliseconds,
                ErrorMessage = result.IsPassed ? null : result.ExceptionMessage,
                IgnoreMessage = result.IgnoreMessage,
            }).ToList();

            testClasses.Add(new TestClassModel
            {
                Name = testClass.ClassName,
                TestDetails = testDetails,
            });
        });

        var result = new TestRun
        {
            TotalTests = testClasses.Sum(c => c.TestDetails.Count),
            PassedTests = testClasses.Sum(c => c.TestDetails.Count(td => td.Status == "Passed")),
            FailedTests = testClasses.Sum(c => c.TestDetails.Count(td => td.Status == "Failed")),
            IgnoredTests = testClasses.Sum(c => c.TestDetails.Count(td => td.Status == "Ignored")),
            TestClasses = testClasses.ToList(),
        };

        return Task.FromResult(result);
    }

    /// <summary>
    /// Saves the test run results to the database.
    /// </summary>
    private async Task SaveTestResultsToDatabase(TestRun? testRunModel)
    {
        dbContext.TestRuns.Add(testRunModel);
        await dbContext.SaveChangesAsync();
    }
}