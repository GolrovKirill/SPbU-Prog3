namespace MatrixMultiply.Tests;

using CreateMatrix;
using MatrixMultiply;
using MatrixMultiply.Exceptions;

/// <summary>
/// Test class.
/// </summary>
public class MultiplicationTest
{
    private static bool EqualsMatricesArray(int[,] matrix1, int[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) ||
            matrix1.GetLength(1) != matrix2.GetLength(1))
        {
            return false;
        }

        for (var i = 0; i < matrix1.GetLength(0); i++)
        {
            for (var j = 0; j < matrix1.GetLength(1); j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool EqualsMatricesArrayAndList(int[,] matrix1, List<List<int>> matrix2)
    {
        if (matrix1.GetUpperBound(0) != matrix2.Count - 1)
        {
            return false;
        }

        var columnCount = matrix1.GetLength(1);
        for (int i = 0; i < matrix2.Count; i++)
        {
            if (matrix2[i].Count != columnCount)
            {
                return false;
            }
        }

        for (var i = 0; i <= matrix1.GetUpperBound(0); i++)
        {
            for (var j = 0; j < columnCount; j++)
            {
                if (matrix1[i, j] != matrix2[i][j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Test create matrix.
    /// </summary>
    [Test]
    public void TestCreate()
    {
        const int countRows = 1;
        const int countColumns = 1;
        const string path = "../../../../MatrixMultiply.Tests/Tests/testCreate.txt";

        File.Delete(path);

        Assert.That(!File.Exists(path));

        Create.CreateFile(path, countRows, countColumns);

        Assert.That(File.Exists(path));
    }

    /// <summary>
    /// Check created matrix on correct.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TestReadCorrect()
    {
        const int countRows = 1;
        const int countColumns = 1;
        const string path = "../../../../MatrixMultiply.Tests/Tests/testCreate.txt";

        var matrix = await ReadFile.ReadFileMatrixAsync(path);

        Assert.That(matrix.Count == countRows && matrix[0].Count == countColumns);
        File.Delete(path);
    }

    /// <summary>
    /// Test create impossible matrix.
    /// </summary>
    [Test]
    public void TestCreateIncorrectCountRowsOrColumns()
    {
        const int countRows = -2;
        const int countColumns = 2;
        const string path = "../../../../MatrixMultiply.Tests/Tests/testCreate.txt";

        Assert.Throws<ArgumentException>(() => Create.CreateFile(path, countRows, countColumns));
    }

    /// <summary>
    /// Test create matrix in incorrect path.
    /// </summary>
    [Test]
    public void TestCreateIncorrectPath()
    {
        const int countRows = 2;
        const int countColumns = 2;
        const string path = "../Incorrect Path/testCreate.txt";

        Assert.Throws<IOException>(() => Create.CreateFile(path, countRows, countColumns));
    }

    /// <summary>
    /// Test read file with exception in count rows.
    /// </summary>
    [Test]
    public void TestReadIncorrectCountRows()
    {
        const string path = "../../../../MatrixMultiply.Tests/Tests/ExceptionRows.txt";

        Assert.ThrowsAsync<IOException>(async () => await ReadFile.ReadFileMatrixAsync(path));
    }

    /// <summary>
    /// Test read file with exception in incorrect symbol.
    /// </summary>
    [Test]
    public void TestReadIncorrectSymbol()
    {
        const string path = "../../../../MatrixMultiply.Tests/Tests/ExceptionSymbol.txt";

        Assert.ThrowsAsync<IOException>(async () => await ReadFile.ReadFileMatrixAsync(path));
    }

    /// <summary>
    /// Test read empty file.
    /// </summary>
    [Test]
    public void TestReadEmpty()
    {
        const string path = "../../../../MatrixMultiply.Tests/Tests/Empty.txt";

        Assert.ThrowsAsync<IOException>(async () => await ReadFile.ReadFileMatrixAsync(path));
    }

    /// <summary>
    /// Test read file incorrect path.
    /// </summary>
    [Test]
    public void TestReadIncorrectPath()
    {
        const string path = "../Incorrect/test1.txt";

        Assert.ThrowsAsync<IOException>(async () => await ReadFile.ReadFileMatrixAsync(path));
    }

    /// <summary>
    /// Test multiplication in single threaded matrices with different dimension.
    /// </summary>
    [Test]
    public void TestIncorrectSingleThreadedMultiplication()
    {
        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test1.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2.txt";
        var matrix1 = ReadFile.ReadFileMatrixAsync(path1);
        var matrix2 = ReadFile.ReadFileMatrixAsync(path2);

        Assert.ThrowsAsync<IncorrectInputMatrix>(async () => await Multiplication.SingleThreadedMultiplication(matrix1, matrix2));
    }

    /// <summary>
    /// Test multiplication in multithreaded matrices with different dimension.
    /// </summary>
    [Test]
    public void TestIncorrectMultiThreadedMultiplication()
    {
        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test1.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2.txt";
        var matrix1 = ReadFile.ReadFileMatrixAsync(path1);
        var matrix2 = ReadFile.ReadFileMatrixAsync(path2);

        Assert.ThrowsAsync<IncorrectInputMatrix>(async () => await Multiplication.MultiThreadedMultiplication(matrix1, matrix2));
    }

    /// <summary>
    /// Test equals results two methods.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TestEqualsSingleThreadedMultiplicationAndMultiThreadedMultiplication()
    {
        for (var i = 1; i <= 50; i++)
        {
            var path = "../../../../MatrixMultiply.Tests/Tests/test" + i + ".txt";
            var matrix = ReadFile.ReadFileMatrixAsync(path);
            var matrixSingle = await Multiplication.SingleThreadedMultiplication(matrix, matrix);
            var matrixMulti = await Multiplication.MultiThreadedMultiplication(matrix, matrix);

            Assert.That(EqualsMatricesArray(matrixMulti, matrixSingle));
        }
    }

    /// <summary>
    /// Test create file in incorrect path.
    /// </summary>
    [Test]
    public void TestIncorrectOutputMatrixFile()
    {
        int[,] matrix =
        {
            { 1, 1 },
            { 1, 1 },
            { 1, 1 },
        };

        const string path = "../../../../IncorrectPath/Tests/test3x2IncorrectPath.txt";

        Assert.Throws<IOException>(() => Multiplication.OutputMatrixFile(path, matrix));
    }

    /// <summary>
    /// Test use OutputMatrixFile.
    /// </summary>
    [Test]
    public void TestCorrectOutputMatrixFile()
    {
        int[,] matrix1 =
        {
            { 1, 1 },
            { 1, 1 },
            { 1, 1 },
        };

        int[,] matrix2 =
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
        };

        int[,] matrixResultMatrix1Matrix2 =
        {
            { 2, 2, 2 },
            { 2, 2, 2 },
            { 2, 2, 2 },
        };

        int[,] matrixResultMatrix2Matrix1 =
        {
            { 3, 3 },
            { 3, 3 },
        };

        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test3x2.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2x3.txt";
        const string path3 = "../../../../MatrixMultiply.Tests/Tests/testRez3x3.txt";
        const string path4 = "../../../../MatrixMultiply.Tests/Tests/testRez2x2.txt";

        Multiplication.OutputMatrixFile(path1, matrix1);
        Multiplication.OutputMatrixFile(path2, matrix2);
        Multiplication.OutputMatrixFile(path3, matrixResultMatrix1Matrix2);
        Multiplication.OutputMatrixFile(path4, matrixResultMatrix2Matrix1);

        Assert.That(File.Exists(path1) && File.Exists(path2) && File.Exists(path3) && File.Exists(path4));
    }

    [Test]
    public async Task TestCorrectMultiplicationUnequalMatrices()
    {
        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test3x2.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2x3.txt";
        const string path3 = "../../../../MatrixMultiply.Tests/Tests/testRez3x3.txt";
        const string path4 = "../../../../MatrixMultiply.Tests/Tests/testRez2x2.txt";

        var matrix1 = ReadFile.ReadFileMatrixAsync(path1);
        var matrix2 = ReadFile.ReadFileMatrixAsync(path2);
        var matrixResultMatrix1Matrix2 = await ReadFile.ReadFileMatrixAsync(path3);
        var matrixResultMatrix2Matrix1 = await ReadFile.ReadFileMatrixAsync(path4);

        var matrixSingleRez1 = await Multiplication.SingleThreadedMultiplication(matrix1, matrix2);
        var matrixMultiRez1 = await Multiplication.MultiThreadedMultiplication(matrix1, matrix2);
        var matrixSingleRez2 = await Multiplication.SingleThreadedMultiplication(matrix2, matrix1);
        var matrixMultiRez2 = await Multiplication.MultiThreadedMultiplication(matrix2, matrix1);

        Assert.Multiple(() =>
        {
            Assert.That(EqualsMatricesArray(matrixSingleRez1, matrixMultiRez1) &&
                        EqualsMatricesArray(matrixSingleRez2, matrixMultiRez2));
            Assert.That(EqualsMatricesArrayAndList(matrixSingleRez1, matrixResultMatrix1Matrix2) &&
                        EqualsMatricesArrayAndList(matrixMultiRez2, matrixResultMatrix2Matrix1));
        });

        File.Delete(path1);
        File.Delete(path2);
        File.Delete(path3);
        File.Delete(path4);
    }
}