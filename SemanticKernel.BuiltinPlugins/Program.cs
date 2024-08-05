using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string bingKey = configuration["Bing:ApiKey"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var bingConnector = new BingConnector(bingKey);
var plugin = new WebSearchEnginePlugin(bingConnector);
kernel.ImportPluginFromObject(plugin, "BingPlugin");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

var results = kernel.InvokePromptStreamingAsync("What is Semantic Kernel from Microsoft?", new KernelArguments(settings));
await foreach (var message in results)
{
    Console.Write(message);
}

Console.WriteLine();
Console.ReadLine();

//manual execution
//IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//// Enable Automatic Function Calling
//OpenAIPromptExecutionSettings executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

//// Initialize chat history
//ChatHistory chatHistory = new();
//chatHistory.AddUserMessage(prompt);

//// Generate and execute a plan
//ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
//Console.WriteLine(result.Content);


#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.