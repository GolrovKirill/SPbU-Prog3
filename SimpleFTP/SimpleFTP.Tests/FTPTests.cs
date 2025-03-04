// <copyright file="FTPTests.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace SimpleFTP.Tests;

using System.Net;

/// <summary>
/// Unit tests for the FTP server functionality.
/// </summary>
public class FTPTests
{
    private const int Port = 1234;

    private const string Host = "localhost";

    private FTPServer? server;

    /// <summary>
    /// Initializes and starts the FTP server before each test.
    /// </summary>
    [SetUp]
    public void StartServer()
    {
        server?.Shutdown();

        server = new FTPServer(IPAddress.Any, Port);
        server.Start();
    }

    [TearDown]
    public void StopServer()
    {
        server?.Shutdown();
        server = null;
    }

    /// <summary>
    /// Tests that a list request for a non-existent directory throws an exception.
    /// </summary>
    [Test]
    public void TestListNonExistentDirectory()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List("../../../Test"));
    }

    /// <summary>
    /// Tests that a get request for a non-existent file path throws an exception.
    /// </summary>
    [Test]
    public void TestGetNonExistentDirectory()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.Get("../../../TestFiles/test0.txt"));
    }

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
    }

    /// <summary>
    /// Tests that a list request with an invalid path format throws an exception.
    /// </summary>
    [Test]
    public void TestListWithInvalidPathFormat()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List("invalid:path/for/list"));
    }

    /// <summary>
    /// Tests that a get request with an invalid file name throws an exception.
    /// </summary>
    [Test]
    public void TestGetWithInvalidFileName()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.Get("invalid|file*name.txt"));
    }

    /// <summary>
    /// Tests that a list request using an empty string as a path throws an exception.
    /// </summary>
    [Test]
    public void TestListWithEmptyPath()
    {
        var client = new FTPClient(Host, Port);
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List(string.Empty));
    }

    /// <summary>
    /// Tests that a list request for a directory that is not a directory throws an exception.
    /// </summary>
    [Test]
    public void TestListNotADirectory()
    {
        var client = new FTPClient(Host, Port);
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await client.List("../../../TestFiles/test1.txt");
        });
    }
}
