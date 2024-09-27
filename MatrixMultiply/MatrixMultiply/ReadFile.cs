using System.Diagnostics.CodeAnalysis;

namespace MatrixMultiply;

using MatrixMultiply.Exceptions;

/// <summary>
/// Opening file and write him to list of lists.
/// </summary>
public static class ReadFile
{
    /// <summary>
    /// Opening file and return matrix.
    /// </summary>
    /// <param name="path">The path to the matrix file.</param>
    /// <returns>A matrix in the form of a list of lists.</returns>
    public static List<List<int>> ReadFileMatrix(string path)
    {
        var lastCountRow = -1;

        var matrixList = new List<List<int>>();
        try
        {
            foreach (var line in File.ReadLines(path))
            {
                var str = line.Split(' ');
                var currentRow = new List<int>();

                foreach (var elementStr in str)
                {
                    var tryParse = int.TryParse(elementStr, out var elementInt);

                    switch (tryParse)
                    {
                        case true when lastCountRow == str.Length || lastCountRow == -1:
                            currentRow.Add(elementInt);
                            break;
                        case true when lastCountRow != str.Length:
                            throw new IncorrectInputMatrix("The matrix has rows of different sizes.");
                            break;
                        case false when lastCountRow == str.Length:
                            throw new IncorrectInputMatrix("The matrix does not contain a number.");
                            break;
                    }
                }

                if (str.Length == 0)
                {
                    throw new IncorrectInputMatrix("The matrix contains an empty row.");
                }

                lastCountRow = str.Length;
                matrixList.Add(currentRow);
            }
        }
        catch (IOException)
        {
            throw new IOException("This file either does not exist, or you specified the wrong path to it.");
        }
        catch (NotSupportedException)
        {
            throw new NotSupportedException("This file does not support this functionality.");
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException("Access error.");
        }

        return matrixList;
    }
}