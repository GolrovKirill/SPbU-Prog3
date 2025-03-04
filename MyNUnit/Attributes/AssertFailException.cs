// <copyright file="AssertFailException.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace Attributes;

/// <summary>
/// Class that implements AssertFailException.
/// </summary>
public class AssertFailException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssertFailException"/> class.
    /// </summary>
    public AssertFailException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertFailException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public AssertFailException(string message)
        : base(message)
    {
    }
}