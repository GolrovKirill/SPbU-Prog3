// <copyright file="FTPClient.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Client class.
/// </summary>
/// <param name="serverAddress">Host name.</param>
/// <param name="serverPort">Port.</param>
public class FTPClient(string serverAddress, int serverPort)
{
    private readonly string host = serverAddress;
    private readonly int port = serverPort;

    /// <summary>
    /// Listing files in a directory on the server.
    /// </summary>
    /// <param name="directoryPath">Path.</param>
    /// <returns>List of contents if a directory.</returns>
    public async Task<(string Path, bool IsDirectory)[]> List(string directoryPath)
    {
        if (File.Exists(directoryPath))
        {
            throw new InvalidOperationException();
        }

        using var tcpClient = new TcpClient(host, port);
        var networkStream = tcpClient.GetStream();
        await using var streamWriter = new StreamWriter(networkStream);
        await streamWriter.WriteLineAsync($"1 {directoryPath}");
        await streamWriter.FlushAsync();
        return await ReceiveListResponse(networkStream);
    }

    /// <summary>
    /// Downloading a file from the server.
    /// </summary>
    /// <param name="filePath">Path.</param>
    /// <returns>The file byte array.</returns>
    public async Task<byte[]> Get(string filePath)
    {
        using var tcpClient = new TcpClient(host, port);
        var networkStream = tcpClient.GetStream();
        await using var streamWriter = new StreamWriter(networkStream);
        await streamWriter.WriteLineAsync($"2 {filePath}");
        await streamWriter.FlushAsync();
        return await ReceiveFileResponse(networkStream);
    }

    private static async Task<(string Path, bool IsDirectory)[]> ReceiveListResponse(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        var responseData = await streamReader.ReadToEndAsync();
        var splitData = responseData.Split(" ");
        if (int.Parse(splitData[0]) == -1)
        {
            throw new DirectoryNotFoundException();
        }

        var responses = new (string Path, bool IsDirectory)[int.Parse(splitData[0])];
        for (int i = 0; i < responses.Length; i++)
        {
            responses[i] = (splitData[(i * 2) + 1], bool.Parse(splitData[(i * 2) + 2]));
        }

        return responses;
    }

    private static async Task<byte[]> ReceiveFileResponse(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        using var streamReader = new StreamReader(stream);

        byte[] sizeBuffer = new byte[8];
        await stream.ReadExactlyAsync(sizeBuffer);
        long fileSize = BitConverter.ToInt64(sizeBuffer, 0);
        if (fileSize == -1)
        {
            throw new DirectoryNotFoundException();
        }

        byte[] buffer = new byte[8192];
        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, bytesRead);
            if (memoryStream.Length >= fileSize)
            {
                break;
            }
        }

        return memoryStream.ToArray();
    }
}