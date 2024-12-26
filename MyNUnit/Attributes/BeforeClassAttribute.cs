// <copyright file="BeforeClassAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Attributes;

/// <summary>
/// Class that implements before class of test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
}
