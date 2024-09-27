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
    /// <param name="matrix1">First matrix in multiplication.</param>
    /// <param name="matrix2">Second matrix in multiplication.</param>
    /// <returns>Matrices in the form of a two-dimensional array.</returns>
    /// <exception cref="IncorrectInputMatrix">Incorrect input matrices.</exception>
    public static int[,] SingleThreadedMultiplication(List<List<int>> matrix1, List<List<int>> matrix2)
    {
        var resultMatrix = new int[matrix1.Count, matrix2[0].Count];

        if (!CheckDimMatrix(matrix1, matrix2))
        {
            throw new IncorrectInputMatrix("Count columns first matrix is not equal count rows second matrix.");
        }

        for (var i = 0; i < matrix1.Count; i++)
        {
            for (var j = 0; j < matrix2[0].Count; j++)
            {
                for (var l = 0; l < matrix1[0].Count; l++)
                {
                    resultMatrix[i, j] += matrix1[i][l] * matrix2[l][j];
                }
            }
        }

        return resultMatrix;
    }

    /// <summary>
    /// Multiplication matrices in some threads.
    /// </summary>
    /// <param name="matrix1">First matrix in multiplication.</param>
    /// <param name="matrix2">Second matrix in multiplication.</param>
    /// <returns>Matrices in the form of a two-dimensional array.</returns>
    /// <exception cref="IncorrectInputMatrix">Incorrect input matrices.</exception>
    public static int[,] MultiThreadedMultiplication(List<List<int>> matrix1, List<List<int>> matrix2)
    {
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
                        for (var m = 0; m < matrix2[0].Count; m++)
                        {
                            resultMatrix[j, l] += matrix1[j][m] * matrix2[m][l];
                        }
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
    /// Outputs the finished matrix to the console.
    /// </summary>
    /// <param name="matrix">Input matrix.</param>
    public static void PrintMatrix(int[,] matrix)
    {
        var rows = matrix.GetUpperBound(0) + 1;
        var columns = matrix.Length / rows;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                Console.Write(matrix[i, j] + " ");
            }

            Console.WriteLine(" ");
        }
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

    private static bool CheckDimMatrix(List<List<int>> matrix1, List<List<int>> matrix2)
    {
        return matrix2.Count == matrix1[0].Count;
    }
}