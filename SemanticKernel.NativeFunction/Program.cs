using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernel.NativeFunction.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("269b90a0-964b-4896-ae6f-6a40fea601dd")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.
    WithAzureOpenAIChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

kernel.ImportFunctions(new UnitedStatesPlugin(), "UnitedStatesPlugin");

var function = kernel.Functions.GetFunction("UnitedStatesPlugin", "GetPopulation");

ContextVariables variables = new ContextVariables
{
    { "input", "2015" }
};

var result = await kernel.RunAsync(
    variables,
    function
);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();