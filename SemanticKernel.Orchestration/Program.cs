using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernel.Plugins.Models;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("38200dae-db69-441e-b03a-86f740caac94")
    .Build();


string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

kernel.ImportPluginFromType<UnitedStatesPlugin>();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "MailPlugin");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "MailPlugin");

//manual function execution
OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

string prompt = @"Write a paragraph to share the population of the United States in 2015. 
Make sure to specify how many people, among the population, identify themselves as male and female. 
Don't share approximations, please share the exact numbers.";

var chatHistory = new ChatHistory();
chatHistory.AddMessage(AuthorRole.User, prompt);

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

////as long as the content is null, it means that the chat completion service is waiting for a function call to be processed
//var functionCalls = ((OpenAIChatMessageContent)result).GetOpenAIFunctionToolCalls();
//foreach (var functionCall in functionCalls)
//{
//    KernelFunction pluginFunction;
//    KernelArguments arguments;
//    kernel.Plugins.TryGetFunctionAndArguments(functionCall, out pluginFunction, out arguments);
//    var functionResult = await kernel.InvokeAsync(pluginFunction!, arguments!);
//    var jsonResponse = functionResult.GetValue<UnitedStatesResponse>();
//    var json = JsonSerializer.Serialize(jsonResponse);
//    Console.WriteLine(json);
//    chatHistory.AddMessage(AuthorRole.Tool, json);
//}

//result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);


//var streamingResult = kernel.InvokePromptStreamingAsync(prompt, new KernelArguments(settings));
//await foreach (var streamingResponse in streamingResult)
//{
//    Console.Write(streamingResponse.InnerContent);
//}

Console.WriteLine(result.Content);

//var result = await kernel.InvokePromptAsync(prompt, new KernelArguments(settings));

//Console.WriteLine(result.GetValue<string>());
Console.ReadLine();