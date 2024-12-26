// <copyright file="AssertFailException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyNUnit;

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