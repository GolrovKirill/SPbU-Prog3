namespace CreateMatrix;

using System.Text;

/// <summary>
/// Creating and writing matrix in file.
/// </summary>
public static class Create
{
    /// <summary>
    /// Writing a matrix to a file with random values with specified parameters.
    /// </summary>
    /// <param name="path">Where to save file.</param>
    /// <param name="countRows">Count rows in matrix.</param>
    /// <param name="countColumns">Count columns in matrix.</param>
    public static void CreateFile(string path, int countRows, int countColumns)
    {
        if (countColumns <= 0 || countRows <= 0)
        {
            throw new ArgumentException("A matrix cannot have fewer than one row or column.");
        }

        var matrix = CreateMatrix(countRows, countColumns);

        var currentString = new StringBuilder();
        var resultStrings = new List<string>();

        for (var i = 0; i < countRows; i++)
        {
            for (var j = 0; j < countColumns; j++)
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

    /// <summary>
    /// Create matrix with random value.
    /// </summary>
    /// <param name="countRows">Count rows in matrix.</param>
    /// <param name="countColumns">Count columns in matrix.</param>
    /// <returns>Matrix.</returns>
    private static int[,] CreateMatrix(int countRows, int countColumns)
    {
        var matrix = new int[countRows, countColumns];
        var random = new Random();

        for (var i = 0; i < countRows; i++)
        {
            for (var j = 0; j < countColumns; j++)
            {
                matrix[i, j] = random.Next(-100, 100);
            }
        }

        return matrix;
    }
}