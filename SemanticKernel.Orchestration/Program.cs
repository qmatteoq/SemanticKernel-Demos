using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("38200dae-db69-441e-b03a-86f740caac94")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];

var kernel = new KernelBuilder()
    .AddAzureOpenAIChatCompletion("gpt-4", "gpt-4", endpoint, apiKey)
    .Build();

kernel.ImportPluginFromType<UnitedStatesPlugin>();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "MailPlugin");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "MailPlugin");

//manual function execution
OpenAIPromptExecutionSettings settings = new()
{
    FunctionCallBehavior = FunctionCallBehavior.EnableKernelFunctions
};

var chatHistory = new ChatHistory();
chatHistory.AddUserMessage("Write a business mail to share the population of the United States in 2015. Then add also, among the population, how many people identified themselves as male.");

var chatCompletionService = kernel.Services.GetService<IChatCompletionService>();
var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);

//as long as the content is null, it means that the chat completion service is waiting for a function call to be processed
var functionCall = ((OpenAIChatMessageContent)result).GetOpenAIFunctionResponse();
while (functionCall != null)
{
    KernelFunction pluginFunction;
    KernelArguments arguments;
    kernel.Plugins.TryGetFunctionAndArguments(functionCall, out pluginFunction, out arguments);
    var functionResult = await kernel.InvokeAsync(pluginFunction!, arguments!);
    Console.WriteLine(functionResult.GetValue<string>());
    chatHistory.AddFunctionMessage(functionResult.GetValue<string>(), functionResult.Function.Name);

    result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    functionCall = ((OpenAIChatMessageContent)result).GetOpenAIFunctionResponse();
}


Console.WriteLine(result.Content);
Console.ReadLine();