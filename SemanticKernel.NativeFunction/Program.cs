using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using SemanticKernel.NativeFunction.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("269b90a0-964b-4896-ae6f-6a40fea601dd")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];

var kernel = new KernelBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, modelId, endpoint, apiKey)
    .Build();


kernel.ImportPluginFromType<UnitedStatesPlugin>();
OpenAIPromptExecutionSettings settings = new()
{
    FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
};

var chatCompletionService = kernel.Services.GetService<IChatCompletionService>();
var result = await chatCompletionService.GetChatMessageContentAsync("Please tell me the population of the United States in 2015", settings, kernel);

//var functionCall = ((OpenAIChatMessageContent)result).GetOpenAIFunctionResponse();
//if (functionCall != null)
//{
//    KernelFunction pluginFunction;
//    KernelArguments arguments;
//    kernel.Plugins.TryGetFunctionAndArguments(functionCall, out pluginFunction, out arguments);
//    var functionResult = await kernel.InvokeAsync(pluginFunction!, arguments!);
//    Console.WriteLine(functionResult.GetValue<string>());
//}


//var function = kernel.Plugins.GetFunction("UnitedStatesPlugin", "GetPopulation");

KernelArguments variables = new KernelArguments(settings)
{
    { "input", "2015" }
};

//var result = await kernel.InvokePromptAsync("Please tell me the population of the United States in {{$input}}", variables);

//var functionCall = result.Function.
//var result = await kernel.InvokeAsync(function, variables);

//Console.WriteLine(result.GetValue<string>());
//Console.ReadLine();