using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using SemanticKernel.Plugins.Plugins.TicketPlugin;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
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
    .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
    .WithAzureAISearchMemoryDb(searchEndpoint, searchApiKey)
    .Build();

//var kernelMemory = new MemoryWebClient("http://localhost:9001");

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentChatName, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "MailPlugin");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "MailPlugin");

var plugin = new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true);
kernel.ImportPluginFromObject(plugin, "memory");

kernel.ImportPluginFromType<TicketPlugin>();

#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var planner = new FunctionCallingStepwisePlanner();

string prompt = @"Get a list of all the tickets about CSS. Then find from the knowledge base using Kernel Memory a list of potential CSS issues and how to fix them.
Finally, draft a professional mail to the team to share the list of tickets and how to potentially fix them and print it on the screen.";

var result = await planner.ExecuteAsync(kernel, prompt);

Console.WriteLine(result.FinalAnswer);
Console.ReadLine();

#pragma warning restore SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
