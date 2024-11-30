namespace MyThreadPool.Tests;

using System.Globalization;

/// <summary>
/// Tests for MyThreadPool functionality.
/// </summary>
public class MyThreadPoolTests
{
    /// <summary>
    /// Verify that an exception is thrown when an invalid number of threads is supplied to the pool.
    /// </summary>
    [Test]
    public void ThrowsArgumentOutOfRangeException_WhenCreatingPoolWithInvalidThreadCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var myThreadPool = new MyThreadPool(-10);
        });
    }

    /// <summary>
    /// Validate that tasks return correct results when executed in the thread pool.
    /// </summary>
    /// <param name="countThreads">Number of Threads.</param>
    /// <param name="countTasks">Number of tasks.</param>
    [TestCase(1, 3)]
    [TestCase(3, 1)]
    [TestCase(2, 2)]
    [TestCase(4, 8)]
    [TestCase(8, 4)]
    public void ExecutesTasks_CorrectlyInThreadPool(int countThreads, int countTasks)
    {
        var tasks = new IMyTask<int>[countTasks];
        var pool = new MyThreadPool(countThreads);
        for (var i = 0; i < countTasks; i++)
        {
            tasks[i] = pool.Submit(() => 1 + 1);
        }

        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.Result, Is.EqualTo(2));
                Assert.That(task.IsCompleted, Is.True);
            });
        }

        pool.Shutdown();
    }

    /// <summary>
    /// Test the ContinueWith functionality of the thread pool.
    /// </summary>
    /// <param name="countThreads">Number of threads.</param>
    /// <param name="countTasks">Number of tasks.</param>
    /// <param name="countContinue">Number of Continuation Tasks.</param>
    [TestCase(1, 1, 1)]
    [TestCase(2, 3, 3)]
    [TestCase(3, 2, 1)]
    [TestCase(2, 2, 1)]
    [TestCase(5, 10, 5)]
    [TestCase(10, 5, 5)]
    public void HandlesContinuationTasks_Properly(int countThreads, int countTasks, int countContinue)
    {
        var tasks = new IMyTask<int>[countTasks];
        var pool = new MyThreadPool(countThreads);
        for (var i = 0; i < countTasks; i++)
        {
            tasks[i] = pool.Submit(() => 1 + 1);
        }

        AssertTasks(tasks, 2);

        var continuationTasks = new IMyTask<string>[countContinue];
        for (var i = 0; i < countContinue; i++)
        {
            continuationTasks[i] = tasks[i].ContinueWith((x) => x.ToString());
        }

        AssertTasks(continuationTasks, "2");
        pool.Shutdown();
    }

    /// <summary>
    /// Test the thread pool functionality with tasks returning different types.
    /// </summary>
    [Test]
    public void ExecutesTasks_WithDifferentReturnTypesSuccessfully()
    {
        const int countThreads = 2;
        var pool = new MyThreadPool(countThreads);
        var task1 = pool.Submit(() => 2 * 3);
        var task2 = pool.Submit(() => "Hello," + " World!");
        var task3 = pool.Submit(() => 1 != 2);
        var task4 = pool.Submit(() =>
        {
            float[] arr = { 1.2f, 3.4f, 5.2f };
            return arr.Sum();
        }).ContinueWith((x) => x * 10);

        Assert.Multiple(() =>
        {
            Assert.That(task1.Result, Is.EqualTo(6));
            Assert.That(task1.IsCompleted, Is.True);
            Assert.That(task2.Result, Is.EqualTo("Hello, World!"));
            Assert.That(task2.IsCompleted, Is.True);
            Assert.That(task3.Result, Is.EqualTo(true));
            Assert.That(task3.IsCompleted, Is.True);
            Assert.That(task4.Result, Is.EqualTo(98));
            Assert.That(task4.IsCompleted, Is.True);
        });
        pool.Shutdown();
    }

    /// <summary>
    /// Test sequential execution with multiple Continuation Tasks.
    /// </summary>
    [Test]
    public void SupportsMultipleContinuationTasks_InSequence()
    {
        const int countThreads = 2;
        var pool = new MyThreadPool(countThreads);
        var task = pool.Submit(() =>
        {
            float[] arr = { 1.2f, 3.4f, 5.2f };
            return arr.Sum();
        }).ContinueWith((x) => x * 10).ContinueWith((x) => x.ToString(CultureInfo.InvariantCulture));

        Assert.Multiple(() =>
        {
            Assert.That(task.Result, Is.EqualTo("98"));
            Assert.That(task.IsCompleted, Is.True);
        });
        pool.Shutdown();
    }

    /// <summary>
    /// Test for handling exceptions thrown during task execution.
    /// </summary>
    /// <exception cref="DivideByZeroException">This exception is expected for testing purposes.</exception>
    [Test]
    public void ThrowsAggregateException_WhenTaskThrowsException()
    {
        const int countThreads = 4;
        var pool = new MyThreadPool(countThreads);
        var task = pool.Submit<int?>(() => throw new DivideByZeroException());
        var exception = Assert.Throws<AggregateException>(() => { var a = task.Result; });

        Assert.Multiple(() =>
        {
            Assert.That(exception.InnerException?.GetType(), Is.EqualTo(typeof(DivideByZeroException)));
            Assert.That(task.IsCompleted, Is.False);
        });
        pool.Shutdown();
    }

    /// <summary>
    /// Test the behavior of the thread pool's shutdown method.
    /// </summary>
    [Test]
    public void SuccessfullyShutsDown_WhenActiveTasksExist()
    {
        const int countThreads = 1;
        var pool = new MyThreadPool(countThreads);
        var task1 = pool.Submit(() =>
        {
            Thread.Sleep(500);
            return 1;
        });
        var task2 = pool.Submit(() => 1 + 1);
        Thread.Sleep(100);
        pool.Shutdown();

        Assert.That(task1.Result, Is.EqualTo(1));
        Assert.Throws<TaskCanceledException>(() => { var a = task2.Result; });
        pool.Shutdown();
    }

    /// <summary>
    /// Test the exception handling when continuing tasks after shutdown.
    /// </summary>
    [Test]
    public void ThrowsTaskCanceledException_WhenContinuingAfterShutdown()
    {
        const int countThreads = 4;
        var pool = new MyThreadPool(countThreads);
        var task1 = pool.Submit(() => 1 + 2);
        pool.Shutdown();
        var task2 = task1.ContinueWith((x) => x.ToString());

        Assert.Throws<TaskCanceledException>(() => { var a = task2.Result; });
        pool.Shutdown();
    }

    private static void AssertTasks<T>(IMyTask<T>[] tasks, T expectedResult)
    {
        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.Result, Is.EqualTo(expectedResult));
                Assert.That(task.IsCompleted, Is.True);
            });
        }
    }
}