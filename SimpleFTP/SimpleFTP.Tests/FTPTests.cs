namespace SimpleFTP.Tests;

using System.Net;

/// <summary>
/// Unit tests for the FTP server functionality.
/// </summary>
public class FTPTests
{
    private const int Port = 1234;

    private const string Host = "localhost";

    private static readonly object[] TestCase =
    [
        new object[]
        {
            "../../../TestFiles",
            new (string path, bool isDir)[]
            {
                ("../../../TestFiles/test2.txt", false),
                ("../../../TestFiles/test1.txt", false),
            },
        },
    ];

    private FTPServer server;

    /// <summary>
    /// Initializes and starts the FTP server before each test.
    /// </summary>
    [SetUp]
    public void StartServer()
    {
        server = new FTPServer(IPAddress.Any, Port);
        server.Start();
    }

    /// <summary>
    /// Tests that a list request for a non-existent directory throws an exception.
    /// </summary>
    [Test]
    public void TestListNonExistentDirectory()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List("../../../Test"));
        server.Shutdown();
    }

    /// <summary>
    /// Tests that a get request for a non-existent file path throws an exception.
    /// </summary>
    [Test]
    public void TestGetNonExistentDirectory()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.Get("../../../TestFiles/test0.txt"));
        server.Shutdown();
    }

    /// <summary>
    /// Tests the response for a list request from the server.
    /// </summary>
    /// <param name="path">The directory path for which to retrieve file listings.</param>
    /// <param name="expectedResult">The expected result containing file paths and their type.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous test execution.</returns>
    [Test]
    [TestCaseSource(nameof(TestCase))]
    public async Task TestList(string path, (string Path, bool IsDir)[] expectedResult)
    {
        var client = new FTPClient(Host, Port);
        var response = await client.List(path);
        Assert.That(response, Has.Length.EqualTo(expectedResult.Length));
        foreach (var element in response)
        {
            Assert.That(expectedResult, Does.Contain(element));
        }

        server.Shutdown();
    }

    /// <summary>
    /// Tests the response when retrieving a file from the server.
    /// </summary>
    /// <param name="path">The path of the file to retrieve.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous test execution.</returns>
    [TestCase("../../../TestFiles/test1.txt")]
    public async Task TestGet(string path)
    {
        var expectedResult = await File.ReadAllBytesAsync(path);
        var client = new FTPClient(Host, Port);
        var response = await client.Get(path);
        Assert.That(response, Is.EqualTo(expectedResult));
        server.Shutdown();
    }
}
