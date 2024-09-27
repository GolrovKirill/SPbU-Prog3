namespace MatrixMultiply.Tests;

using CreateMatrix;
using MatrixMultiply;
using MatrixMultiply.Exceptions;

/// <summary>
/// Test class.
/// </summary>
public class MultiplicationTest
{
    private static bool EqualsMatrices(int[,] matrix1, int[,] matrix2)
    {
        if (matrix1.GetUpperBound(0) != matrix2.GetUpperBound(0) ||
            matrix1.Length / (matrix1.GetUpperBound(0) + 1) != matrix2.Length / (matrix2.GetUpperBound(0) + 1))
        {
            return false;
        }

        for (var i = 0; i < matrix1.GetUpperBound(0); i++)
        {
            for (var j = 0; j < matrix1.Length / (matrix1.GetUpperBound(0) + 1); j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
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

        Assert.That(!File.Exists(path));

        Create.CreateFile(path, countRows, countColumns);

        Assert.That(File.Exists(path));
    }

    /// <summary>
    /// Check created matrix on correct.
    /// </summary>
    [Test]
    public void TestReadCorrect()
    {
        const int countRows = 1;
        const int countColumns = 1;
        const string path = "../../../../MatrixMultiply.Tests/Tests/testCreate.txt";

        var matrix = ReadFile.ReadFileMatrix(path);

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

        Assert.Throws<IOException>(() => ReadFile.ReadFileMatrix(path));
    }

    /// <summary>
    /// Test read file with exception in incorrect symbol.
    /// </summary>
    [Test]
    public void TestReadIncorrectSymbol()
    {
        const string path = "../../../../MatrixMultiply.Tests/Tests/ExceptionSymbol.txt";

        Assert.Throws<IOException>(() => ReadFile.ReadFileMatrix(path));
    }

    /// <summary>
    /// Test read empty file.
    /// </summary>
    [Test]
    public void TestReadEmpty()
    {
        const string path = "../../../../MatrixMultiply.Tests/Tests/Empty.txt";

        Assert.Throws<IOException>(() => ReadFile.ReadFileMatrix(path));
    }

    /// <summary>
    /// Test read file incorrect path.
    /// </summary>
    [Test]
    public void TestReadIncorrectPath()
    {
        const string path = "../Incorrect/test1.txt";

        Assert.Throws<IOException>(() => ReadFile.ReadFileMatrix(path));
    }

    /// <summary>
    /// Test multiplication in single threaded matrices with different dimension.
    /// </summary>
    [Test]
    public void TestIncorrectSingleThreadedMultiplication()
    {
        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test1.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2.txt";
        var matrix1 = ReadFile.ReadFileMatrix(path1);
        var matrix2 = ReadFile.ReadFileMatrix(path2);

        Assert.Throws<IncorrectInputMatrix>(() => Multiplication.SingleThreadedMultiplication(matrix1, matrix2));
    }

    /// <summary>
    /// Test multiplication in multithreaded matrices with different dimension.
    /// </summary>
    [Test]
    public void TestIncorrectMultiThreadedMultiplication()
    {
        const string path1 = "../../../../MatrixMultiply.Tests/Tests/test1.txt";
        const string path2 = "../../../../MatrixMultiply.Tests/Tests/test2.txt";
        var matrix1 = ReadFile.ReadFileMatrix(path1);
        var matrix2 = ReadFile.ReadFileMatrix(path2);

        Assert.Throws<IncorrectInputMatrix>(() => Multiplication.MultiThreadedMultiplication(matrix1, matrix2));
    }

    /// <summary>
    /// Test equals results two methods.
    /// </summary>
    [Test]
    public void TestEqualsSingleThreadedMultiplicationAndMultiThreadedMultiplication()
    {
        for (var i = 1; i <= 50; i++)
        {
            var path = "../../../../MatrixMultiply.Tests/Tests/test" + i + ".txt";
            var matrix = ReadFile.ReadFileMatrix(path);
            var matrixSingle = Multiplication.SingleThreadedMultiplication(matrix, matrix);
            var matrixMulti = Multiplication.MultiThreadedMultiplication(matrix, matrix);

            Assert.That(EqualsMatrices(matrixMulti, matrixSingle));
        }
    }
}