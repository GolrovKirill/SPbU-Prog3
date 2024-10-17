namespace Lazy.Tests;

using System.Threading;
using static System.Environment;

public class LazyTest
{
    private const double Equal = 0.0000001;

    private static bool IsDoubleType(object o)
    {
        switch (Type.GetTypeCode(o.GetType()))
        {
            case TypeCode.Double:
            case TypeCode.Decimal:
                return true;
            default:
                return false;
        }
    }

    private static bool EqualArray(object[] array)
    {
        if (IsDoubleType(array[0]))
        {
            var arrayDoubles = new double[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                arrayDoubles[i] = (double)array[i];
            }

            if (arrayDoubles.Any(element => Math.Abs(element - arrayDoubles[0]) > Equal))
            {
                return false;
            }
        }
        else
        {
            if (array.Any(element => element != array[0]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checking that SingleLazy return ArgumentNullException if function return null.
    /// </summary>
    [Test]
    public void TestSingleThreadNullException()
    {
        var lazy = new SingleLazy<int?>(() => null);
        Assert.Throws<ArgumentNullException>(() => lazy.Get());
    }

    /// <summary>
    /// Checking the condition under which SingleLazy will work once.
    /// </summary>
    [Test]
    public void CheckSingleLazyCondition()
    {
        const double firstElement = 25453454;
        const double secondElement = 56734223;

        var lazy = new SingleLazy<double>(() => (secondElement / firstElement));

        Assert.That(!lazy.FlagResult);
        var res1 = lazy.Get();

        Assert.That(lazy.FlagResult);
        var res2 = lazy.Get();

        Assert.That(Math.Abs(res1 - res2), Is.LessThan(Equal));
    }

    /// <summary>
    /// Checking the condition that MultipleLazy works with double types.
    /// </summary>
    [Test]
    public void CheckMultipleLazyWithDouble()
    {
        var countThreads = ProcessorCount;
        var threads = new Thread[countThreads];
        var results = new object[countThreads];

        for (var j = 0; j < 10000; j++)
        {
            var firstElement = 134.345 * j;
            var secondElement = 123.34 * j;
            var lazy = new MultipleLazy<double>(() => (firstElement % secondElement));

            for (var i = 0; i < countThreads; i++)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    results[localI] = lazy.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(EqualArray(results));
        }
    }

    /// <summary>
    /// Checking the condition that MultipleLazy works with string types.
    /// </summary>
    [Test]
    public void CheckMultipleLazyWithString()
    {
        var countThreads = ProcessorCount;
        var threads = new Thread[countThreads];
        var results = new object[countThreads];

        for (var j = 0; j < 10000; j++)
        {
            const string inputString = "Hi ";

            for (var i = 0; i < countThreads; i++)
            {
                var localI = i;
                var lazy = new MultipleLazy<string>(() => inputString + localI.ToString());
                threads[i] = new Thread(() =>
                {
                    results[localI] = lazy.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(!EqualArray(results));
        }
    }

    /// <summary>
    /// Checking that MultipleLazy is running in multithreaded mode.
    /// </summary>
    [Test]
    public void CheckMultipleLazy()
    {
        const int answer = 56;
        var array = new int[] { 4, 1, 5, 8, 4, 1, 9, 7, 6, 11 };
        var threads = new Thread[ProcessorCount];
        var chankSize = (array.Length / threads.Length) + 1;
        var results = new int[threads.Length];

        int Supplier()
        {
            for (var i = 0; i < threads.Length; ++i)
            {
                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = localI * chankSize; j < (localI + 1) * chankSize && j < array.Length; ++j)
                    {
                        results[localI] += array[j];
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            var result = results.Sum();
            return result;
        }

        var lazy = new MultipleLazy<int>(Supplier);
        Assert.That(lazy.Get(), Is.EqualTo(answer));
    }
}