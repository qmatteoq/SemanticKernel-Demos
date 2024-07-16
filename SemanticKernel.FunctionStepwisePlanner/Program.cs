using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;
using Microsoft.SemanticKernel.Planning;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("7c81681b-0553-4bfc-985e-b84345deffe8")
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

#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var planner = new FunctionCallingStepwisePlanner();

var ask = @"Write a business mail to share the population of the United States in 2015. 
Make sure to specify how many people, among the population, identify themselves as male and female.
Don't share approximations, please share the exact numbers.";

var result = await planner.ExecuteAsync(kernel, ask);

Console.WriteLine(result.FinalAnswer);
Console.ReadLine();

#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.