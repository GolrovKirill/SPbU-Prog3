// <copyright file="TestApp.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace TestApp;

using Attributes;

public class TestApp
{
    [BeforeClass]
    public static void SetupClass()
    {
    }

    [Before]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That(true);
    }

    [Test]
    public void FailedTest()
    {
        Assert.That(false);
    }

    [Test(expectedException: typeof(InvalidOperationException))]
    public void ThrowsExpectedException()
    {
        throw new InvalidOperationException();
    }

    [Test]
    public void ThrowsUnexpectedException()
    {
        throw new ArgumentException();
    }

    [Test(ignoreMessage: "This test is ignored")]
    public void IgnoredTest()
    {
    }

    [After]
    public void Teardown()
    {
    }

    [AfterClass]
    public static void TeardownClass()
    {
    }
}

public class AnotherTestClass
{
    [BeforeClass]
    public static void AnotherSetupClass()
    {
    }

    [Before]
    public void AnotherSetup()
    {
    }

    [Test]
    public void AnotherTest1()
    {
        Assert.That(true);
    }

    [After]
    public void AnotherTeardown()
    {
    }

    [AfterClass]
    public static void AnotherTeardownClass()
    {
    }
}

public class EmptyTestClass
{
}