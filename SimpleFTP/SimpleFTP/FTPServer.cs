// <copyright file="FTPServer.cs" company="Gorlov Kirill">
// Copyright (c) Gorlov Kirill. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.
// https://github.com/GolrovKirill/SPbU-Prog3/blob/main/LICENSE
// </copyright>
namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Server class.
/// </summary>
/// <param name="ipAddress">Ip address.</param>
/// <param name="portNumber">Port.</param>
public class FTPServer(IPAddress ipAddress, int portNumber)
{
    private readonly TcpListener tcpListener = new(ipAddress, portNumber);
    private readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Starts the server.
    /// </summary>
    public void Start()
    {
        tcpListener.Start();
        RunAsync();
    }

    /// <summary>
    /// Shuts down the server.
    /// </summary>
    public void Shutdown()
    {
        cancellationTokenSource.Cancel();
        tcpListener.Stop();
    }

    private async void RunAsync()
    {
        List<Task> clientTasks = [];
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var clientSocket = await tcpListener.AcceptSocketAsync(cancellationTokenSource.Token);
                var task = Task.Run(async () =>
                {
                    await using var networkStream = new NetworkStream(clientSocket);
                    using var streamReader = new StreamReader(networkStream);
                    var requestLine = await streamReader.ReadLineAsync();
                    if (requestLine == null)
                    {
                        return;
                    }

                    try
                    {
                        await HandleRequest(requestLine, networkStream);
                    }
                    finally
                    {
                        clientSocket.Close();
                    }
                });
                clientTasks.Add(task);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        await Task.WhenAll(clientTasks);
    }

    private async Task HandleRequest(string requestLine, Stream stream)
    {
        var components = requestLine.Split(' ');
        if (components.Length != 2)
        {
            return;
        }

        switch (components[0])
        {
            case "1":
                await SendDirectoryList(components[1], stream);
                break;
            case "2":
                await SendFile(components[1], stream);
                break;
            default:
                throw new ArgumentException();
        }
    }

    private async Task SendDirectoryList(string directoryPath, Stream stream)
    {
        await using var streamWriter = new StreamWriter(stream);
        if (!Directory.Exists(directoryPath))
        {
            await streamWriter.WriteLineAsync("-1");
            await streamWriter.FlushAsync();
            return;
        }

        var entries = Directory.GetFileSystemEntries(directoryPath);
        StringBuilder responseBuilder = new();
        responseBuilder.Append(entries.Length);

        foreach (var entry in entries)
        {
            responseBuilder.Append($" {entry.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}" +
                $" {Directory.Exists(entry)}");
        }

        await streamWriter.WriteLineAsync(responseBuilder.ToString());
        await streamWriter.FlushAsync();
    }

    private async Task SendFile(string filePath, Stream stream)
    {
        if (!File.Exists(filePath))
        {
            await stream.WriteAsync(BitConverter.GetBytes(-1L), 0, sizeof(long));
            return;
        }

        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        long fileLength = fileStream.Length;
        await stream.WriteAsync(BitConverter.GetBytes(fileLength), 0, sizeof(long));

        var buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await stream.WriteAsync(buffer, 0, bytesRead);
        }
    }
}