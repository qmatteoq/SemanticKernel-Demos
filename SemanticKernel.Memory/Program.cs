using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("2d112f3a-9cf4-4b55-931e-474661e9d70d")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentChatName = configuration["AzureOpenAI:DeploymentChatName"];
string deploymentEmbeddingName = configuration["AzureOpenAI:DeploymentEmbeddingName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

string searchApiKey = configuration["AzureSearch:ApiKey"];
string searchEndpoint = configuration["AzureSearch:Endpoint"];

var embeddingConfig = new AzureOpenAIConfig
{
    APIKey = apiKey,
    Deployment = deploymentEmbeddingName,
    Endpoint = endpoint,
    APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
    Auth = AzureOpenAIConfig.AuthTypes.APIKey
};

var chatConfig = new AzureOpenAIConfig
{
    APIKey = apiKey,
    Deployment = deploymentChatName,
    Endpoint = endpoint,
    APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
    Auth = AzureOpenAIConfig.AuthTypes.APIKey
};

var kernelMemory = new KernelMemoryBuilder()
    .WithAzureOpenAITextGeneration(chatConfig)
    .WithAzureOpenAIEmbeddingGeneration(embeddingConfig)
    .WithAzureAISearch(searchEndpoint, searchApiKey)
    .Build();

var kernel = new KernelBuilder()
    .WithAzureOpenAIChatCompletionService(deploymentChatName, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");
kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "AskPlugin");

kernel.ImportFunctions(new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true), "memory");

var planner = new ActionPlanner(kernel);

var ask = @"Answer the question 'What is Contoso Electronics? Use your memories to find the answer, then use it to share the information via mail";
var originalPlan = await planner.CreatePlanAsync(ask);
var originalPlanResult = await kernel.RunAsync(originalPlan);

Console.WriteLine(originalPlanResult.GetValue<string>());
Console.ReadLine();