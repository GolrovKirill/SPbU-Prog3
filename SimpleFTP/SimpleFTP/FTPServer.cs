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
    private readonly CancellationTokenSource clt = new();

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
        clt.Cancel();
        tcpListener.Stop();
    }

    private async void RunAsync()
    {
        List<Task> clientTasks = new();
        while (!clt.Token.IsCancellationRequested)
        {
            try
            {
                var clientSocket = await tcpListener.AcceptSocketAsync(clt.Token);
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
                SendFile(components[1], stream);
                break;
            default:
                throw new ArgumentException();
        }
    }

    private static async Task SendDirectoryList(string directoryPath, Stream stream)
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

    private static void SendFile(string filePath, Stream stream)
    {
        using var binaryWriter = new BinaryWriter(stream);
        if (!File.Exists(filePath))
        {
            binaryWriter.Write(-1L);
            binaryWriter.Flush();
            return;
        }

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        binaryWriter.Write(fileStream.Length);

        var buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            binaryWriter.Write(buffer, 0, bytesRead);
        }

        binaryWriter.Flush();
    }
}