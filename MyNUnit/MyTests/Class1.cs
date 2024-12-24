namespace MyTests;

using MyNUnit;

public class ClassForTesting
{
    [BeforeClass]
    public static void BeforeClassMethod()
    {
        Console.WriteLine($"Before class");
    }

    [AfterClass]
    public static void AfterClassMethod()
    {
        Console.WriteLine("After class");
    }

    [Before]
    public static void BeforeMethod()
    {
        Console.WriteLine("Before test");
    }

    [After]
    public static void AfterMethod()
    {
        Console.WriteLine("After test");
    }

    [My]
    public static void TestPassed()
    {
        MyAssert.IsTrue(true);
    }

    [My(typeof(Exception), "Ignore")]
    public static void TestIgnored()
    {
        MyAssert.IsTrue(true);
    }

    [My]
    public static void TestFailed()
    {
        MyAssert.IsTrue(false);
    }

    [My(typeof(Exception))]
    public static void TestException()
    {
        MyAssert.Throws<Exception>(() => throw new Exception());
    }
}
