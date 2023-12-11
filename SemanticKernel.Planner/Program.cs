using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Handlebars;
using SemanticKernel.Plugins.Plugins.UnitedStatesPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("4ef9f3ca-be0f-43ef-9cb0-eac313050d99")
    .Build();


string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];

string openAIApiKey = configuration["OpenAI:ApiKey"];
string openAIModelId = configuration["OpenAI:Model"];

var kernel = new KernelBuilder()
    .AddOpenAIChatCompletion(openAIModelId, openAIApiKey)
    .AddOpenAITextGeneration(openAIModelId, openAIApiKey)   
    //.AddAzureOpenAIChatCompletion(deploymentName, modelId, endpoint, apiKey)
    //.AddAzureOpenAITextGeneration(deploymentName, modelId, endpoint, apiKey)
    .Build();


kernel.ImportPluginFromType<UnitedStatesPlugin>();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory + "//MailPlugin", "MailPlugin");

#pragma warning disable SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
//var planner = new HandlebarsPlanner(new HandlebarsPlannerConfig()
//{
//    AllowLoops = true
//});

var planner = new FunctionCallingStepwisePlanner();

string ask = "Write a mail to share the population of the United States in 2015";


//var plan = await planner.CreatePlanAsync(kernel, ask);
var result = await planner.ExecuteAsync(kernel, ask);
//var result = plan.Invoke(kernel, new KernelArguments());

Console.WriteLine(result);
Console.ReadLine();

#pragma warning restore SKEXP0061 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.