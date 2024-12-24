namespace MyNUnit.Tests;

using NUnit.Framework;
using MyNUnit;

public class Tests
{
    [Test]
    public async Task TestRunsTestsCorrectly()
    {
        var res = await Tester.RunTestsAsync("../../../../MyTests/bin/Debug/net8.0");

        var expected = new List<MyTestResult>
        {
            new ("TestPassed", "Passed", ""),
            new ("TestIgnored", "Ignored", "ignore"),
            new ("TestFailed", "Failed", "Exception has been thrown by the target of an invocation."),
            new ("TestException", "Passed", "")
        };

        for (var i = 0; i < res.Count; ++i)
        {
            Assert.That(Comparer(res[i], expected[i]), Is.True);
        }
    }

    [Test]
    public static void TestThrowsDirectoryNotFoundException()
    {
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await Tester.RunTestsAsync("../Test"));
    }

    private static bool Comparer(MyTestResult actual, MyTestResult expected)
    {
        return actual.Name == expected.Name && actual.Message == expected.Message && actual.Status == expected.Status;
    }
}