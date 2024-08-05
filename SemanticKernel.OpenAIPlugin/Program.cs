using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();


#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

const string pluginManifestUrl = "https://semantickernel-unitedstatesdata.azurewebsites.net/api/.well-known/ai-plugin.json";
await kernel.ImportPluginFromOpenAIAsync("UnitedStatesPlugin", new Uri(pluginManifestUrl));

var function = kernel.Plugins.GetFunction("UnitedStatesPlugin", "GetPopulation");

KernelArguments variables = new KernelArguments
{
    { "year", "2020" }
};

var result = await kernel.InvokeAsync(function, variables);

Console.WriteLine(result.GetValue<RestApiOperationResponse>().Content);
Console.ReadLine();

#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.