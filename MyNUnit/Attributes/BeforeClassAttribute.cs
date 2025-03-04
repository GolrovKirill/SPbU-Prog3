// <copyright file="BeforeClassAttribute.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace Attributes;

/// <summary>
/// Class that implements before class of test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
}
