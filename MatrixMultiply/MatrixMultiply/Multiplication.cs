// <copyright file="Multiplication.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace MatrixMultiply;

using System.Text;
using MatrixMultiply.Exceptions;

/// <summary>
/// Multiplication  matrices and writing finished matrix in file.
/// </summary>
public static class Multiplication
{
    /// <summary>
    /// Multiplication matrices in one thread.
    /// </summary>
    /// <param name="matrix1Task">First matrix in multiplication.</param>
    /// <param name="matrix2Task">Second matrix in multiplication.</param>
    /// <returns>Matrices in the form of a two-dimensional array.</returns>
    /// <exception cref="IncorrectInputMatrix">Incorrect input matrices.</exception>
    public static async Task<int[,]> SingleThreadedMultiplication(Task<List<List<int>>> matrix1Task, Task<List<List<int>>> matrix2Task)
    {
        var matrix1 = await matrix1Task;
        var matrix2 = await matrix2Task;
        var resultMatrix = new int[matrix1.Count, matrix2[0].Count];

        if (!CheckDimMatrix(matrix1, matrix2))
        {
            throw new IncorrectInputMatrix("Count columns first matrix is not equal count rows second matrix.");
        }

        for (var i = 0; i < matrix1.Count; i++)
        {
            for (var j = 0; j < matrix2[0].Count; j++)
            {
                resultMatrix[i, j] = Enumerable.Range(0, matrix1[0].Count).Sum(l => matrix1[i][l] * matrix2[l][j]);
            }
        }

        return resultMatrix;
    }

    /// <summary>
    /// Multiplication matrices in some threads.
    /// </summary>
    /// <param name="matrix1Task">First matrix in multiplication.</param>
    /// <param name="matrix2Task">Second matrix in multiplication.</param>
    /// <returns>Matrices in the form of a two-dimensional array.</returns>
    /// <exception cref="IncorrectInputMatrix">Incorrect input matrices.</exception>
    public static async Task<int[,]> MultiThreadedMultiplication(Task<List<List<int>>> matrix1Task, Task<List<List<int>>> matrix2Task)
    {
        var matrix1 = await matrix1Task;
        var matrix2 = await matrix2Task;
        var resultMatrix = new int[matrix1.Count, matrix2[0].Count];
        var threads = new Thread[Math.Min(Environment.ProcessorCount, matrix1.Count)];
        var chunkSize = (matrix1.Count / threads.Length) + 1;

        if (!CheckDimMatrix(matrix1, matrix2))
        {
            throw new IncorrectInputMatrix("Count columns first matrix is not equal count rows second matrix.");
        }

        for (var i = 0; i < threads.Length; ++i)
        {
            var localI = i;

            threads[i] = new Thread(() =>
            {
                for (var j = localI * chunkSize; j < (localI + 1) * chunkSize && j < matrix1.Count; j++)
                {
                    for (var l = 0; l < matrix2[0].Count; l++)
                    {
                        var sum = 0;
                        for (var m = 0; m < matrix2.Count; m++)
                        {
                           sum += matrix1[j][m] * matrix2[m][l];
                        }

                        resultMatrix[j, l] = sum;
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    /// <summary>
    /// Output finished matrix in file.
    /// </summary>
    /// <param name="path">Where to save file.</param>
    /// <param name="matrix">Finished matrix.</param>
    public static void OutputMatrixFile(string path, int[,] matrix)
    {
        var currentString = new StringBuilder();
        var resultStrings = new List<string>();
        var rows = matrix.GetUpperBound(0) + 1;
        var columns = matrix.Length / rows;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                currentString.Append(matrix[i, j] + " ");
            }

            resultStrings.Add(currentString.ToString()[..^1]);
            currentString.Clear();
        }

        try
        {
            File.WriteAllLines(path, resultStrings);
        }
        catch
        {
            throw new IOException("Incorrect path.");
        }
    }

    private static bool CheckDimMatrix(List<List<int>> matrix1, List<List<int>> matrix2) => matrix1[0].Count == matrix2.Count;
}