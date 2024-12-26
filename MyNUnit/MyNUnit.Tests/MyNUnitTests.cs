// <copyright file="MyNUnitTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit.Tests;

using MyNUnit;

public class Tests
{
    private readonly Dictionary<string, (bool, double, string)> expectedResult = new()
    {
        { "TestFallAfterAssert", (false, 0.3 , "Failed: Exception of type 'MyNUnit.AssertFailException' was thrown.") },
        { "TestFalledWithException", (false, 0.2, "Failed: This test should fall") },
        { "IgnoredTestPassed", (false, 0.2, "Failed: This test should be ignored") },
        { "ExpectedExceptionPassed", (true, 2.5, "Passed with expected exception: Operation is not valid due to the current state of the object.") },
        { "AssertWithTrueExpressionPassed", (true, 0.2, "Passed") },
        { "BeforeMethodWasCalled", (true, 0.2, "Passed") },
    };

    /// <summary>
    /// Test run tests by path in the directory.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public Task TestOfTests_PathToDirectory()
    {
        var runner = new TestRunner();
        var result = runner.RunTest("../../../../TestApp//bin/Debug/net9.0");

        foreach (var testResult in result)
        {
            Assert.That(testResult.TestName != null);

            if (testResult.TestName == null)
            {
                continue;
            }

            var expected = expectedResult[testResult.TestName];

            Assert.That(testResult.Passed == expected.Item1);
            Assert.That(Math.Abs(testResult.Duration.TotalSeconds - (expected.Item2 / 1000)) <= 1);
            Assert.That(testResult.Message == expected.Item3);
        }

        return Task.CompletedTask;
    }
}