using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using SemanticKernel.Plugins.Plugins.TicketPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("d696f8b6-b14f-4f1e-acf2-2567451363c6")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

kernel.ImportPluginFromType<TicketPlugin>();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Prompts");
var prompts = kernel.CreatePluginFromPromptDirectory(pluginsDirectory);
kernel.ImportPluginFromFunctions("prompts", prompts);

// automatic function calling

#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var planner = new FunctionCallingStepwisePlanner();

string prompt = "Get a list of all the tickets about CSS. Then write a professional mail to the team to share their current status.";

var result = await planner.ExecuteAsync(kernel, prompt);

Console.WriteLine(result.FinalAnswer);
Console.ReadLine();

#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

