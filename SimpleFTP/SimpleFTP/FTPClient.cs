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
    public async Task<(string path, bool isDirectory)[]> List(string directoryPath)
    {
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
        return ReceiveFileResponse(networkStream);
    }

    private static async Task<(string path, bool isDirectory)[]> ReceiveListResponse(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        var responseData = await streamReader.ReadToEndAsync();
        var splitData = responseData.Split(" ");
        if (int.Parse(splitData[0]) == -1)
        {
            throw new DirectoryNotFoundException();
        }

        var responses = new (string path, bool isDirectory)[int.Parse(splitData[0])];
        for (int i = 0; i < responses.Length; i++)
        {
            responses[i] = (splitData[(i * 2) + 1], bool.Parse(splitData[(i * 2) + 2]));
        }

        return responses;
    }

    private static byte[] ReceiveFileResponse(Stream stream)
    {
        using var binaryReader = new BinaryReader(stream);
        var fileSize = binaryReader.ReadInt64();
        if (fileSize == -1)
        {
            throw new DirectoryNotFoundException();
        }

        var fileData = new byte[fileSize];
        for (long i = 0; i < fileSize; i++)
        {
            fileData[i] = binaryReader.ReadByte();
        }

        return fileData;
    }
}