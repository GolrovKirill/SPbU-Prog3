// <copyright file="Assert.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace Attributes;

/// <summary>
/// Class that checks the validity of the condition.
/// </summary>
public static class Assert
{
    /// <summary>
    /// Method that throws exception if condition is false.
    /// </summary>
    /// <param name="condition">What we check for accuracy.</param>
    public static void That(bool condition)
    {
        if (!condition)
        {
            throw new AssertFailException();
        }
    }
}