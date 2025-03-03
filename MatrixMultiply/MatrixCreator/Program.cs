for (var i = 1; i < 51; i++)
{
    var path = "../../../../MatrixMultiply.Tests/Tests/test" + i + ".txt";
    MatrixCreator.MatrixCreator.CreateFileWithMatrix(path, i + 1, i + 1);
}
