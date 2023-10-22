using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Functions.OpenAPI.Extensions;
using Microsoft.SemanticKernel.Orchestration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("38200dae-db69-441e-b03a-86f740caac94")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.
    WithAzureChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

const string pluginManifestUrl = "http://localhost:7098/api/.well-known/ai-plugin.json";
await kernel.ImportPluginFunctionsAsync("UnitedStatesPlugin", new Uri(pluginManifestUrl));

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");

var mailFunction = kernel.Functions.GetFunction("MailPlugin", "WriteBusinessMail");
var populationFunction = kernel.Functions.GetFunction("UnitedStatesPlugin", "GetPopulation");

ContextVariables variables = new ContextVariables
{
    { "year", "2018" }
};

var result = await kernel.RunAsync(
    variables,
    populationFunction,
    mailFunction
);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();