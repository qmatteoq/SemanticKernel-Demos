using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("4ef9f3ca-be0f-43ef-9cb0-eac313050d99")
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
var planner = new HandlebarsPlanner();

var ask = "Write a mail to share the number of the United States population in 2015 for a research program.";

HandlebarsPlan plan;

if (!File.Exists("plan.txt"))
{
    // Create the plan
    plan = await planner.CreatePlanAsync(kernel, ask);
    Console.WriteLine(plan);

    var serializedPlan = plan.ToString();
    await File.WriteAllTextAsync("plan.txt", serializedPlan);
}
else
{
    string serializedPlan = await File.ReadAllTextAsync("plan.txt");
    plan = new HandlebarsPlan(serializedPlan);
}

// Execute the plan
var originalPlanResult = await plan.InvokeAsync(kernel);
Console.WriteLine(originalPlanResult);
#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
