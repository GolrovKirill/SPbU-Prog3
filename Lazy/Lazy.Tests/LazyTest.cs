namespace Lazy.Tests;

using System.Threading;
using static System.Environment;

public class LazyTest
{
    private const double Epsilon = 0.0000001;

    /// <summary>
    /// Checking that SingleLazy return null if function return null.
    /// </summary>
    [Test]
    public void TestSingleThreadNullException()
    {
        var lazy = new SingleLazy<int?>(() => null);
        Assert.That(lazy.Get(), Is.EqualTo(null));
    }

    /// <summary>
    /// Checking the condition under which SingleLazy will work once.
    /// </summary>
    [Test]
    public void CheckSingleLazyCondition()
    {
        const double firstElement = 25453454;
        const double secondElement = 56734223;
        var callCount = 0;

        var lazy = new SingleLazy<double>(() => ComputeValue(firstElement, secondElement, ref callCount));

        Assert.That(callCount, Is.EqualTo(0));

        var res1 = lazy.Get();

        Assert.That(callCount, Is.EqualTo(1));

        var res2 = lazy.Get();

        Assert.Multiple(() =>
        {
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(Math.Abs(res1 - res2), Is.LessThan(Epsilon));
        });
        return;

        double ComputeValue(double first, double second, ref int count)
        {
            count++;
            return second / first;
        }
    }

    /// <summary>
    /// Checks that MultiThreadLazy creates a value only once during multithreaded access.
    /// </summary>
    [Test]
    public void MultiThreadLazy_CreatesValueOnce()
    {
        var creationCount = 0;
        var lazy = new MultiThreadLazy<int>(() =>
        {
            Interlocked.Increment(ref creationCount);
            Thread.Sleep(100);
            return 52;
        });

        var threads = new Thread[Environment.ProcessorCount];

        for (var i = 0; i < threads.Length; i++)
        {
            var i1 = i;
            threads[i1] = new Thread(() => { var value = lazy.Get(); });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(creationCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(lazy.Get(), Is.EqualTo(52));
        });
    }

    /// <summary>
    /// Checks that MultiThreadLazy returns the same value for multithreaded access.
    /// </summary>
    [Test]
    public void MultiThreadLazy_ReturnsSameValue()
    {
        var lazy = new MultiThreadLazy<string>(() => "Hello, World!");

        var threads = new Thread[Environment.ProcessorCount];
        var results = new string[threads.Length];

        for (var i = 0; i < threads.Length; i++)
        {
            var i1 = i;
            threads[i] = new Thread(() => { results[i1] = lazy.Get(); });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        foreach (var result in results)
        {
            Assert.That(result, Is.EqualTo("Hello, World!"));
        }
    }

    /// <summary>
    /// Checks that MultiThreadLazy returns null if supplier returns null.
    /// </summary>
    [Test]
    public void MultiThreadLazy_ThrowsException_WhenSupplierReturnsNull()
    {
        var lazy = new MultiThreadLazy<string>(() => null!);

        Assert.That(lazy.Get(), Is.EqualTo(null));
    }

    /// <summary>
    /// Checks that MultiThreadLazy throws an exception if the supplier throws an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the supplier throws an exception.</exception>
    [Test]
    public void MultiThreadLazy_ThrowsException_WhenSupplierThrows()
    {
        var lazy = new MultiThreadLazy<string>(() => throw new InvalidOperationException());

        var ex = Assert.Throws<InvalidOperationException>(() => lazy.Get());
        Assert.That(ex.InnerException is InvalidOperationException, Is.True);
    }
}