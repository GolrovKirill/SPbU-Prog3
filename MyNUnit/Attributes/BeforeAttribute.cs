// <copyright file="BeforeAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Attributes;

/// <summary>
/// Class that implements Before test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
}
