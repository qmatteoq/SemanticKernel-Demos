using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Planners;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("e3d5349a-9eb9-45db-821b-03e4d2c37173")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string bingKey = configuration["Bing:ApiKey"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.
    WithAzureChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

var bingConnector = new BingConnector(bingKey);
kernel.ImportFunctions(new WebSearchEnginePlugin(bingConnector), "BingPlugin");

var pluginsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "AskPlugin");

var planner = new StepwisePlanner(kernel);

var ask = "Who is the prime minister of UK?";
var originalPlan = planner.CreatePlan(ask);
var originalPlanResult = await kernel.RunAsync(originalPlan);

Console.WriteLine(originalPlanResult.GetValue<string>());
Console.ReadLine();