using Attributes;

namespace MyNUnit;

using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;

/// <summary>
/// Class that implements running tests.
/// </summary>
public class TestRunner
    {
        private readonly ConcurrentBag<TestResult> _testsResult = [];

        /// <summary>
        /// Run tests for each assemblies multi-threaded and print results.
        /// </summary>
        /// <param name="path">Path to the directory with tests.</param>
        public ConcurrentBag<TestResult> RunTest(string path)
        {
            var assemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom).ToList();

            var tasks = assemblies.Select(assembly => Task.Run(() => ExecuteTests(assembly))).ToArray();
            Task.WaitAll(tasks);

            PrintResults();
            return _testsResult;
        }

        /// <summary>
        /// Executes tests for assembly.
        /// </summary>
        /// <param name="assembly">Assembly with tests.</param>
        private void ExecuteTests(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var testMethods = type.GetMethods().Where(m => m.GetCustomAttribute<TestAttribute>() is not null).ToList();

                if (testMethods.Count != 0)
                {
                    var instance = Activator.CreateInstance(type)!;

                ExecuteBeforeClass(type);

                    foreach (var method in testMethods)
                    {
                        ExecuteBefore(instance);
                        ExecuteTest(instance, method);
                        ExecuteAfter(instance);
                    }

                    ExecuteAfterClass(type);
                }
            }
        }

        /// <summary>
        /// Execute methods before test starts.
        /// </summary>
        /// <param name="type">Some class with test attribute.</param>
        private static void ExecuteBeforeClass(Type type)
        {
            var beforeClassMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<BeforeClassAttribute>() is not null);

            foreach (var method in beforeClassMethods)
            {
                method.Invoke(null, null);
            }
        }

        /// <summary>
        /// Execute methods after all tests.
        /// </summary>
        /// <param name="type">Some class with test attribute.</param>
        private static void ExecuteAfterClass(Type type)
        {
            var afterClassMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<AfterClassAttribute>() is not null);

            foreach (var method in afterClassMethods)
            {
                method.Invoke(null, null);
            }
        }

        /// <summary>
        /// Execute methods before each test.
        /// </summary>
        /// <param name="instance">Instance of some class.</param>
        private static void ExecuteBefore(object instance)
        {
            var beforeMethods = instance!.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<BeforeAttribute>() is not null);

            foreach (var method in beforeMethods)
            {
                method.Invoke(instance, null);
            }
        }

        /// <summary>
        /// Execute methods after each test.
        /// </summary>
        /// <param name="instance">Instance of some class.</param>
        private static void ExecuteAfter(object instance)
        {
            var afterMethods = instance!.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<AfterAttribute>() is not null);

            foreach (var method in afterMethods)
            {
                method.Invoke(instance, null);
            }
        }

        /// <summary>
        /// Execute test method.
        /// </summary>
        /// <param name="instance">Instance of some class.</param>
        /// <param name="method">Test method.</param>
        private void ExecuteTest(object instance, MethodInfo method)
        {
            var testAttribute = method.GetCustomAttribute<TestAttribute>();
            var result = new TestResult { TestName = method.Name };
            if (testAttribute?.Ignore != null)
            {
                result.Passed = true;
                result.Message = $"Ignored: {testAttribute.Ignore}";
                _testsResult.Add(result);
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                method.Invoke(instance, null);
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
                result.Passed = true;
                result.Message = "Passed";
            }
            catch (TargetInvocationException ex)
            {
                if (testAttribute?.Expected is not null && ex.InnerException?.GetType() == testAttribute.Expected)
                {
                    result.Passed = true;
                    result.Message = $"Passed with expected exception: {ex.InnerException.Message}";
                }
                else
                {
                    result.Passed = false;
                    result.Message = $"Failed: {ex.InnerException?.Message}";
                }
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Message = $"Failed: {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
                _testsResult.Add(result);
            }
        }

        /// <summary>
        /// Method that print results of tests.
        /// </summary>
        public void PrintResults()
        {
            Console.WriteLine("======== Test Results ========");
            foreach (var result in _testsResult)
            {
                Console.WriteLine($"{result.TestName}, {(result.Passed)}, {result.Duration.TotalMilliseconds}, {result.Message}");
            }
        }
    }
