// <copyright file="AfterAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Attributes;

/// <summary>
/// Class that implements After test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : Attribute
{
}
