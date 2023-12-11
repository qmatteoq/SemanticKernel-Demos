using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Services.


var configuration = new ConfigurationBuilder()
    .AddUserSecrets("d6a28a11-60a1-48f7-b334-15064483b85b")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];

var kernel = new KernelBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, modelId, endpoint, apiKey)
    .Build();

const string pluginManifestUrl = "https://semantickernel-unitedstatesdata.azurewebsites.net/api/.well-known/ai-plugin.json";
await kerne kernel.ImportOpenAIPluginFunctionsAsync("UnitedStatesPlugin", new Uri(pluginManifestUrl));


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