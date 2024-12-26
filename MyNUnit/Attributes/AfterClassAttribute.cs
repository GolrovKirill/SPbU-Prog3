// <copyright file="AfterClassAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Attributes;

/// <summary>
/// Class that implements after class of test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute
{
}
