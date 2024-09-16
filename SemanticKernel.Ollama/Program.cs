using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

var chatService = new OllamaChatCompletionService(modelId: "phi3", endpoint: new Uri("http://localhost:11434"));

var chatHistory = new ChatHistory("You are a helpful assistant that knows about AI.");

string message = string.Empty;
while (message != "stop")
{
    message = Console.ReadLine();
    if (message != "stop")
    {
        chatHistory.AddUserMessage(message);
        var reply = await chatService.GetChatMessageContentAsync(chatHistory);
        chatHistory.AddAssistantMessage(reply.Content);
        Console.WriteLine(reply.Content);
    }
}
