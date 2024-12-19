namespace KR3;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Class for create chat with server and user.
/// </summary>
public class ChatApp
{
    /// <summary>
    /// Starts a chat between the client and the server.
    /// </summary>
    /// <param name="args">The message being transmitted, or connection to the port from the server and user side.</param>
    public void Start(string[] args)
    {
        switch (args.Length)
        {
            case 0:
                Console.WriteLine("Usage: <port> | <ip> <port>");
                break;
            case 1 when int.TryParse(args[0], out int port):
                StartServer(port);
                break;
            case 1:
                Console.WriteLine("Error: The port must be a number.");
                break;
            case 2:
            {
                var ipAddress = args[0];
                if (int.TryParse(args[1], out var port))
                {
                    StartClient(ipAddress, port);
                }
                else
                {
                    Console.WriteLine("Error: The port must be a number.");
                }

                break;
            }
        }

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    private void StartServer(int port)
    {
        var server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"The server is running on the port {port}.");

        var client = server.AcceptTcpClient();
        Console.WriteLine("The client is connected.");

        var stream = client.GetStream();
        var receiveThread = new Thread(() => ReceiveMessages(stream));
        receiveThread.Start();

        SendMessages(stream);

        stream.Close();
        client.Close();
        server.Stop();
    }

    private void StartClient(string ipAddress, int port)
    {
        try
        {
            TcpClient client = new TcpClient(ipAddress, port);
            Console.WriteLine($"Connecting to the server {ipAddress}:{port}.");

            NetworkStream stream = client.GetStream();
            Thread receiveThread = new Thread(() => ReceiveMessages(stream));
            receiveThread.Start();

            SendMessages(stream);

            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
        }
    }

    private void SendMessages(NetworkStream stream)
    {
        var message = Console.ReadLine();
        while (true)
        {
            if (message.ToLower() == "exit")
            {
                break;
            }

            var data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

    private void ReceiveMessages(NetworkStream stream)
    {
        var buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {message}");
        }
    }
}