using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Functions.OpenAPI.Extensions;
using Microsoft.SemanticKernel.Functions.OpenAPI.Model;
using Microsoft.SemanticKernel.Orchestration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("d6a28a11-60a1-48f7-b334-15064483b85b")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.
    WithAzureChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

const string pluginManifestUrl = "http://localhost:7078/api/.well-known/ai-plugin.json";
await kernel.ImportPluginFunctionsAsync("UnitedStatesPlugin", new Uri(pluginManifestUrl));

var function = kernel.Functions.GetFunction("UnitedStatesPlugin", "GetPopulation");

ContextVariables variables = new ContextVariables
{
    { "year", "2020" }
};

var result = await kernel.RunAsync(
    variables,
    function
);

Console.WriteLine(result.GetValue<RestApiOperationResponse>().Content);
Console.ReadLine();