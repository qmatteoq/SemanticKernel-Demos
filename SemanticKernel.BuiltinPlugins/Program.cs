using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("5b68dc4b-5ae4-44c4-a65b-6ae334716c74")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string bingKey = configuration["Bing:ApiKey"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

#pragma warning disable SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var bingConnector = new BingConnector(bingKey);
var plugin = new WebSearchEnginePlugin(bingConnector);
kernel.ImportPluginFromObject(plugin, "BingPlugin");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
};

//var results = kernel.InvokePromptStreamingAsync("What is Semantic Kernel from Microsoft?", new KernelArguments(settings));
//await foreach (var message in results)
//{
//    Console.Write(message);
//}   

//Console.WriteLine();
//Console.ReadLine();


var chatHistory = new ChatHistory();
chatHistory.AddMessage(AuthorRole.User, "What is Semantic Kernel from Microsoft?");

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

var functionCalls = ((OpenAIChatMessageContent)result).GetOpenAIFunctionToolCalls();
foreach (var functionCall in functionCalls)
{
    KernelFunction pluginFunction;
    KernelArguments arguments;
    kernel.Plugins.TryGetFunctionAndArguments(functionCall, out pluginFunction, out arguments);
    var functionResult = await kernel.InvokeAsync(pluginFunction!, arguments!);
    var jsonResponse = functionResult.GetValue<object>();
    var json = JsonSerializer.Serialize(jsonResponse);
    Console.WriteLine(json);
    chatHistory.AddMessage(AuthorRole.Tool, json);
}

result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

Console.WriteLine(result.Content);


#pragma warning restore SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.