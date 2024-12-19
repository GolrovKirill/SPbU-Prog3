namespace KR3.Tests;
using KR3;

public class ChatTests
{
    private ChatApp chat;

    [SetUp]
    public void Setup()
    {
        chat = new ChatApp();
    }

    [Test]
    public void SendMessage_ShouldAddMessageToChat()
    {
        const string message = "Hello, World!";

        chat.SendMessage(message);

        Assert.AreEqual(1, chat.Messages.Count);
        Assert.AreEqual(message, chat.Messages[0]);
    }

    [Test]
    public void ReceiveMessage_ShouldReturnMessage()
    {
        const string message = "Hello, World!";
        chat.SendMessage(message);

        var receivedMessage = chat.ReceiveMessage();

        Assert.AreEqual(message, receivedMessage);
    }

    [Test]
    public void ReceiveMessage_NoMessages_ShouldReturnNull()
    {
        var receivedMessage = chat.ReceiveMessage();

        Assert.IsNull(receivedMessage);
    }
}