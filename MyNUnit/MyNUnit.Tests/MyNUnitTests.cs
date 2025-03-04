// <copyright file="MyNUnitTests.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnitTests;

using Attributes;
using MyNUnit;
using Assert = NUnit.Framework.Assert;

[TestFixture]
public class MyNUnitTests
{
    private const string TestProjectPath = @"../../../../TestApp/bin";
    private MyNUnit testRunner;

    [SetUp]
    public void InitializeTestRunner()
    {
        testRunner = new MyNUnit();
    }

    [TearDown]
    public void CleanupTestRunner()
    {
        testRunner.Dispose();
    }

    [NUnit.Framework.Test]
    public async Task VerifyPassingTests()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);
        Assert.That(testResults, Is.Not.Null);

        Assert.That(testResults, Is.Not.Empty);

        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");
        Assert.That(testClass, Is.Not.Null);

        var testMethod = testClass.TestResults.FirstOrDefault(m => m.TestName == "Test1");

        Assert.Multiple(() =>
        {
            Assert.That(testMethod, Is.Not.Null);
            Assert.That(testMethod.IsPassed, Is.True);
            Assert.That(testMethod.ExceptionMessage, Is.Null);
            Assert.That(testMethod.IgnoreMessage, Is.Null);
        });
    }

    [NUnit.Framework.Test]
    public async Task VerifyIgnoredTest()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);

        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");
        var testMethod = testClass.TestResults.FirstOrDefault(m => m.TestName == "IgnoredTest");

        Assert.That(testMethod, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testMethod.IsPassed, Is.False);
            Assert.That(testMethod.IgnoreMessage, Is.EqualTo("This test is ignored"));
        });
    }

    [NUnit.Framework.Test]
    public async Task VerifyExpectedExceptions()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);

        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");

        var testMethod = testClass.TestResults.FirstOrDefault(t => t.TestName == "ThrowsExpectedException");
        Assert.That(testMethod, Is.Not.Null);
        Assert.That(testMethod.IsPassed, Is.True);
    }

    [NUnit.Framework.Test]
    public async Task VerifyFailingTests()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);

        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");
        var testMethod = testClass.TestResults.FirstOrDefault(m => m.TestName == "FailedTest");

        Assert.That(testMethod, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testMethod.IsPassed, Is.False);
            Assert.That(testMethod.ExceptionType, Is.EqualTo(typeof(AssertFailException)));
        });
    }

    [NUnit.Framework.Test]
    public async Task VerifyUnexpectedException()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);

        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");
        var testMethod = testClass.TestResults.FirstOrDefault(m => m.TestName == "ThrowsUnexpectedException");

        Assert.That(testMethod, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(testMethod.IsPassed, Is.False);
            Assert.That(testMethod.ExceptionType, Is.EqualTo(typeof(ArgumentException)));
        });
    }

    [NUnit.Framework.Test]
    public async Task VerifyMultipleTestClasses()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);
        Assert.That(testResults, Is.Not.Null);

        var testClasses = testResults.Where(r => r.ClassName == "TestApp" || r.ClassName == "AnotherTestClass").ToList();
        Assert.That(testClasses, Has.Count.EqualTo(2));
    }

    [NUnit.Framework.Test]
    public async Task VerifyMixedPassFailTests()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);
        var testClass = testResults.FirstOrDefault(r => r.ClassName == "TestApp");

        var passingTest = testClass.TestResults.FirstOrDefault(m => m.TestName == "Test1");
        var failingTest = testClass.TestResults.FirstOrDefault(m => m.TestName == "FailedTest");

        Assert.Multiple(() =>
        {
            Assert.That(passingTest.IsPassed, Is.True);
            Assert.That(failingTest.IsPassed, Is.False);
        });
    }

    [NUnit.Framework.Test]
    public async Task VerifyEmptyTestClass()
    {
        var testResults = await testRunner.ExecuteTestsAsync(TestProjectPath);
        var testClass = testResults.FirstOrDefault(r => r.ClassName == "EmptyTestClass");

        Assert.That(testClass, Is.Not.Null);
        Assert.That(testClass.TestResults, Is.Empty);
    }
}