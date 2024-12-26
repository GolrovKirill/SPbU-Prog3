using Attributes;
using MyNUnit;
 
public class SomeClass
{
    private int _value;

    [BeforeClass]
    public static void BeforeClass()
    {
        Console.WriteLine("SetUp before tests");
    }

    [Before]
    public void Before()
    {
        Console.WriteLine("Method before test");
        _value = 5;
    }

    [After]
    public static void After()
    {
        Console.WriteLine("Method after test");
    }

    [AfterClass]
    public static void AfterClass()
    {
        Console.WriteLine("TearDown after tests");
    }

    [Test]
    public void BeforeMethodWasCalled()
    {
        if (_value != 5)
        {
            throw new Exception("Before method wasn't called");
        }
    }

    [Test]
    public void AssertWithTrueExpressionPassed()
    {
        Assert.That(_value == 5);
    }

    [Test(typeof(InvalidOperationException))]
    public static void ExpectedExceptionPassed()
    {
        throw new InvalidOperationException();
    }

    [Test("Ignored because of some reason")]
    public static void IgnoredTestPassed()
    {
        throw new Exception("This test should be ignored");
    }

    [Test]
    public static void TestFalledWithException()
    {
        throw new Exception("This test should fall");
    }

    [Test]
    public static void TestFallAfterAssert()
    {
        Assert.That(5 == 6);
    }
}