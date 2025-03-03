// <copyright file="MultiThreadLazy.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>

namespace Lazy;

/// <inheritdoc />
public class MultiThreadLazy<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T? result;
    private volatile bool isCalculated;
    private Exception? cachedException;
    private readonly object lockObject = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Transmitted function.</param>
    public MultiThreadLazy(Func<T> supplier)
        => this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));

    /// <inheritdoc/>
    public T? Get()
    {
        if (isCalculated)
        {
            if (cachedException != null)
            {
                throw cachedException;
            }

            return result;
        }

        lock (lockObject)
        {
            if (!isCalculated)
            {
                try
                {
                    result = supplier();
                    cachedException = null;
                }
                catch (Exception ex)
                {
                    cachedException = new InvalidOperationException("Error while executing supplier function.", ex);
                }
                finally
                {
                    isCalculated = true;
                    supplier = null;
                }
            }
        }

        if (cachedException != null)
        {
            throw cachedException;
        }

        return result;
    }
}