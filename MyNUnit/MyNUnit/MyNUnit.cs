// <copyright file="MyNUnit.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnit;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using Attributes;

/// <summary>
/// Class for executing unit tests.
/// </summary>
public class MyNUnit : IDisposable
{
    private readonly List<AssemblyLoadContext> assemblyContexts = [];

    /// <summary>
    /// Execute tests for all classes in the specified path.
    /// </summary>
    /// <param name="assemblyPath">Path to the assemblies.</param>
    /// <returns>List of test results.</returns>
    public async Task<List<TestResultClass>> ExecuteTestsAsync(string assemblyPath)
    {
        var classTestDictionary = new ConcurrentDictionary<Type, (Type ClassType, FunctionCollection MethodSet)>();
        var discoveredClasses = RetrieveClassesFromAssemblies(assemblyPath).ToList();

        PopulateClassDictionary(discoveredClasses, classTestDictionary);

        var results = new List<TestResultClass>();

        var originalOutput = Console.Out;
        await using var outputWriter = new StringWriter();
        Console.SetOut(outputWriter);

        var testTasks = classTestDictionary.Values.Select(async entry =>
            await RunClassTestsAsync(entry.ClassType, entry.MethodSet));

        results.AddRange(await Task.WhenAll(testTasks));

        Console.SetOut(originalOutput);
        classTestDictionary.Clear();

        await Task.WhenAll(testTasks);
        UnloadAssemblyContexts();

        return results;
    }

    /// <summary>
    /// Display the results of the tests to the console.
    /// </summary>
    public static void DisplayTestResults(List<TestResultClass> results)
    {
        foreach (var classResult in results)
        {
            Console.WriteLine($"Class: {classResult.ClassName}");
            Console.WriteLine(new string('-', 50));

            foreach (var testResult in classResult.TestResults)
            {
                Console.WriteLine($"Test: {testResult.TestName}");

                if (!string.IsNullOrEmpty(testResult.IgnoreMessage))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Status: IGNORED");
                    Console.WriteLine($"Reason: {testResult.IgnoreMessage}");
                }
                else if (testResult.IsPassed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Status: PASSED");
                    Console.WriteLine($"Execution Time: {testResult.Time.TotalMilliseconds} ms");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Status: FAILED");
                    if (testResult.ExceptionType != null)
                    {
                        Console.WriteLine($"Exception Type: {testResult.ExceptionType}");
                        Console.WriteLine($"Exception Message: {testResult.ExceptionMessage}");
                    }
                }

                Console.ResetColor();
                Console.WriteLine(new string('-', 50));
            }
        }

        Console.WriteLine("\n==================== TEST ====================\n");
    }

    /// <summary>
    /// Dispose of resources.
    /// </summary>
    public void Dispose() => UnloadAssemblyContexts();

    /// <summary>
    /// Runs tests for a specified class.
    /// </summary>
    /// <param name="classType">The type of the class containing the tests.</param>
    /// <returns>The results of running the tests.</returns>
    private async Task<TestResultClass> RunClassTestsAsync(Type classType, FunctionCollection methodSet)
    {
        var testOutcomes = new List<TestResult>();
        var beforeAllSuccess = await ExecuteStaticMethodsAsync(methodSet.BeforeAllFunctions);

        if (beforeAllSuccess)
        {
            var testExecutionTasks = methodSet.TestingFunctions.Select(testMethod =>
                RunTestMethodAsync(classType, testMethod, methodSet));

            testOutcomes.AddRange(await Task.WhenAll(testExecutionTasks));
        }
        else
        {
            foreach (var testMethod in methodSet.TestingFunctions)
            {
                testOutcomes.Add(new TestResult(testMethod.Name, false, null, "BeforeClass failed", null, TimeSpan.Zero));
            }
        }

        var afterAllSuccess = await ExecuteStaticMethodsAsync(methodSet.AfterAllFunctions);
        if (!afterAllSuccess)
        {
            testOutcomes.Clear();
            foreach (var testMethod in methodSet.TestingFunctions)
            {
                testOutcomes.Add(new TestResult(testMethod.Name, false, null, "AfterClass failed", null, TimeSpan.Zero));
            }
        }

        return new TestResultClass(classType.Name, testOutcomes);
    }

    /// <summary>
    /// Executes the static methods for the class.
    /// </summary>
    /// <param name="methodQueue">The queue of methods to execute.</param>
    private async Task<bool> ExecuteStaticMethodsAsync(ConcurrentQueue<MethodInfo> methodQueue)
    {
        var methodTasks = methodQueue.Select(staticMethod => Task.Run(() =>
        {
            try
            {
                staticMethod.Invoke(null, null);
                return true;
            }
            catch
            {
                return false;
            }
        }))
        .ToList();

        var taskResults = await Task.WhenAll(methodTasks);
        return taskResults.All(result => result);
    }

    /// <summary>
    /// Executes the specified test method.
    /// </summary>
    /// <param name="testMethod">The test method to execute.</param>
    /// <param name="methodSet">The methods of the test class.</param>
    private Task<TestResult> RunTestMethodAsync(Type classType, MethodInfo testMethod, FunctionCollection methodSet)
    {
        return Task.Run(() =>
        {
            var timer = new Stopwatch();
            var testAttribute = testMethod.GetCustomAttribute<TestAttribute>();

            object? classInstance = Activator.CreateInstance(classType);
            if (classInstance is null)
            {
                throw new InvalidOperationException();
            }

            if (testAttribute.IsIgnore)
            {
                Console.WriteLine($"{testMethod.Name}");
                return new TestResult(
                    testMethod.Name,
                    false,
                    null,
                    null,
                    testAttribute.IgnoreMessage,
                    timer.Elapsed);
            }

            string? exceptionDetails = null;
            Type? exceptionType = null;
            var testPassed = false;

            try
            {
                foreach (var setupMethod in methodSet.SetupFunctions)
                {
                    try
                    {
                        setupMethod.Invoke(classInstance, null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Setup method {setupMethod.Name} failed: {ex.Message}", ex);
                    }
                }

                try
                {
                    timer.Start();
                    testMethod.Invoke(classInstance, null);
                    timer.Stop();

                    testPassed = testAttribute?.ExpectedException == null;
                }
                catch (Exception ex)
                {
                    timer.Stop();
                    exceptionType = ex.GetBaseException().GetType();
                    exceptionDetails = ex.GetBaseException().Message;

                    testPassed = exceptionType == testAttribute?.ExpectedException;
                }
            }
            catch (Exception testExecEx)
            {
                exceptionDetails = testExecEx.Message;
                exceptionType = testExecEx.GetType();
            }
            finally
            {
                foreach (var teardownMethod in methodSet.TearDownFunctions)
                {
                    try
                    {
                        teardownMethod.Invoke(classInstance, null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error in Teardown method {teardownMethod.Name}: {ex}");
                    }
                }
            }

            return new TestResult(testMethod.Name, testPassed, exceptionType, exceptionDetails, null, timer.Elapsed);
        });
    }

    /// <summary>
    /// Retrieves a list of file paths to assemblies from the specified directory.
    /// </summary>
    /// <returns>List of file paths to the assemblies.</returns>
    private IEnumerable<string> FetchAssemblyFilePaths(string assemblyPath)
        => Directory.GetFiles(assemblyPath, "*.dll", SearchOption.AllDirectories)
            .Where(file => !file.Contains("MyNUnit.dll"))
            .Select(Path.GetFullPath);

    /// <summary>
    /// Loads assemblies from the specified paths.
    /// </summary>
    /// <returns>List of loaded assemblies.</returns>
    private List<Assembly> LoadAssemblies(string assemblyPath)
    {
        var assemblyPaths = FetchAssemblyFilePaths(assemblyPath);
        var loadedAssemblies = new List<Assembly>();

        foreach (var path in assemblyPaths)
        {
            var context = new AssemblyLoadContext(path, isCollectible: true);
            var assembly = context.LoadFromAssemblyPath(path);

            loadedAssemblies.Add(assembly);
            assemblyContexts.Add(context);
        }

        return loadedAssemblies;
    }

    /// <summary>
    /// Unloads all loaded assembly contexts.
    /// </summary>
    private void UnloadAssemblyContexts()
    {
        foreach (var context in assemblyContexts)
        {
            context.Unload();

            for (var i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        assemblyContexts.Clear();
    }

    /// <summary>
    /// Retrieves all classes exported from the assemblies.
    /// </summary>
    /// <returns>List of class types.</returns>
    private IEnumerable<Type> RetrieveClassesFromAssemblies(string assemblyPath)
    {
        return LoadAssemblies(assemblyPath)
            .SelectMany(a => a.ExportedTypes)
            .Where(t => t.IsClass);
    }

    /// <summary>
    /// Adds classes to a dictionary.
    /// </summary>
    private void PopulateClassDictionary(
        IEnumerable<Type> classTypes,
        ConcurrentDictionary<Type, (Type ClassType, FunctionCollection MethodSet)> classDictionary)
    {
        foreach (var classType in classTypes)
        {
            classDictionary.TryAdd(classType, (classType, new FunctionCollection(classType)));
        }
    }
}
