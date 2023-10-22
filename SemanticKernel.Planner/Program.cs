using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Functions.OpenAPI.Extensions;
using Microsoft.SemanticKernel.Planners;


var configuration = new ConfigurationBuilder()
    .AddUserSecrets("4ef9f3ca-be0f-43ef-9cb0-eac313050d99")
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

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");

var planner = new StepwisePlanner(kernel);

var ask = "Write a mail to share the number of the United States population in 2015 for a research program.";
var originalPlan = planner.CreatePlan(ask);
var originalPlanResult = await kernel.RunAsync(originalPlan);

Console.WriteLine(originalPlanResult.GetValue<string>());
Console.ReadLine();