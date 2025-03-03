// <copyright file="ILazy.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>

namespace Lazy;

public interface ILazy<out T>
{
    /// <summary>
    /// Returns the result of the function passed to Lazy.
    /// </summary>
    /// <returns>Result function.</returns>
    T? Get();
}