// <copyright file="FunctionCollection.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Attributes;

/// <summary>
/// Class that serves as a repository for methods annotated with specific custom attributes.
/// </summary>
public class FunctionCollection
{
    /// <summary>
    /// Gets or sets a queue that contains methods decorated with the <see cref="TestingAttribute"/>.
    /// These methods are designated for testing purposes.
    /// </summary>
    public ConcurrentQueue<MethodInfo> TestingFunctions { get; set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Gets or sets a queue that holds methods tagged with the <see cref="SetupAttribute"/>.
    /// These methods are executed prior to each test case within the test class.
    /// </summary>
    public ConcurrentQueue<MethodInfo> SetupFunctions { get; set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Gets or sets a queue that stores methods annotated with the <see cref="TearDownAttribute"/>.
    /// These methods are invoked following each test case in the test class.
    /// </summary>
    public ConcurrentQueue<MethodInfo> TearDownFunctions { get; set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Gets or sets a queue for methods marked with the <see cref="BeforeAllAttribute"/>.
    /// These methods run once before executing any of the test methods.
    /// </summary>
    public ConcurrentQueue<MethodInfo> BeforeAllFunctions { get; set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Gets or sets a queue of methods denoted by the <see cref="AfterAllAttribute"/>.
    /// These methods run once after all test methods in the test context have been executed.
    /// </summary>
    public ConcurrentQueue<MethodInfo> AfterAllFunctions { get; set; } = new ConcurrentQueue<MethodInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionCollection"/> class.
    /// This constructor organizes methods based on their custom attributes within the specified <see cref="Type"/>.
    /// </summary>
    public FunctionCollection(Type type)
    {
        var methodsList = type.GetMethods();

        foreach (var function in methodsList)
        {
            if (function.GetParameters().Length != 0 || function.ReturnType != typeof(void))
            {
                continue;
            }

            // Analyze methods with attributes
            var attributeList = function.GetCustomAttributes();

            if (attributeList.OfType<TestAttribute>().Any())
            {
                EnqueueFunction(function, TestingFunctions, false);
            }

            if (attributeList.OfType<BeforeAttribute>().Any())
            {
                EnqueueFunction(function, SetupFunctions, false);
            }

            if (attributeList.OfType<AfterAttribute>().Any())
            {
                EnqueueFunction(function, TearDownFunctions, false);
            }

            if (attributeList.OfType<BeforeClassAttribute>().Any())
            {
                EnqueueFunction(function, BeforeAllFunctions, true);
            }

            if (attributeList.OfType<AfterClassAttribute>().Any())
            {
                EnqueueFunction(function, AfterAllFunctions, true);
            }
        }
    }

    /// <summary>
    /// Validates and adds the method to the specified queue.
    /// </summary>
    private static void EnqueueFunction(
        MethodInfo function,
        ConcurrentQueue<MethodInfo> queue,
        bool isStaticRequired)
    {
        if (isStaticRequired && !function.IsStatic)
        {
            return;
        }

        if (!isStaticRequired && function.IsStatic)
        {
            return;
        }

        queue.Enqueue(function);
    }
}
