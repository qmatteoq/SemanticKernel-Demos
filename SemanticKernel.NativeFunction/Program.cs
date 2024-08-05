using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
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

var function = kernel.Plugins.GetFunction("UnitedStatesPlugin", "GetPopulation");

KernelArguments variables = new KernelArguments()
{
    { "year", "2015" }
};

var result = await kernel.InvokeAsync(function, variables);

Console.WriteLine(result.GetValue<UnitedStatesResponse>().TotalNumber);
Console.ReadLine();