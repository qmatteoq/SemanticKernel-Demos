using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Plugins.Models;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

kernel.ImportPluginFromType<UnitedStatesPlugin>();

string prompt = @"Write a paragraph to share the population of the United States in 2015. 
Make sure to specify how many people, among the population, identify themselves as male and female. 
Don't share approximations, please share the exact numbers.";

//manual function execution
//IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

//// Enable Automatic Function Calling
//OpenAIPromptExecutionSettings executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

//// Initialize chat history
//ChatHistory chatHistory = new();
//chatHistory.AddUserMessage(prompt);

//// Generate and execute a plan
//ChatMessageContent result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, kernel);
//Console.WriteLine(result.Content);

// automatic function calling

AzureOpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

var streamingResult = kernel.InvokePromptStreamingAsync(prompt, new KernelArguments(settings));
await foreach (var streamingResponse in streamingResult)
{
    Console.Write(streamingResponse);
}

Console.ReadLine();