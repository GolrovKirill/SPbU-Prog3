// See https://aka.ms/new-console-template for more information

using CreateMatrix;

for (var i = 1; i < 51; i++)
{
    var path = "../../../../MatrixMultiply.Tests/Tests/test" + i + ".txt";
    Create.CreateFile(path, i + 1, i + 1);
}
