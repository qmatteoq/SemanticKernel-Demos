using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;


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

var searchConfig = new AzureAISearchConfig
{
    APIKey = searchApiKey,
    Endpoint = searchEndpoint,
    UseHybridSearch = true,
    Auth = AzureAISearchConfig.AuthTypes.APIKey
};

var kernelMemory = new KernelMemoryBuilder()
    .WithAzureOpenAITextGeneration(chatConfig)
    .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
    .WithAzureAISearchMemoryDb(searchEndpoint, searchApiKey)
    .Build();

//var kernelMemory = new MemoryWebClient("http://127.0.0.1:9001");

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentChatName, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "MailPlugin");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "MailPlugin");

var plugin = new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true);
kernel.ImportPluginFromObject(plugin, "memory");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

//chat experience

var chatHistory = new ChatHistory();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    var message = Console.ReadLine();

    var prompt = $@"
            The following message contains a question and, optionally, a task to perform on the answer. 
            First ask the question to Kernel Memory, then perform the requested task. Use any plugin you might need to complete the task.
            If Kernel Memory doesn't know the answer, say 'I don't know'.
            If the question is about a topic that isn't convered by the information in Kernel Memory, say 'I can't answer that'.

            Question: {message}

            ";


    chatHistory.AddMessage(AuthorRole.User, prompt);
    var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    Console.WriteLine(result.Content);
    chatHistory.AddMessage(AuthorRole.Assistant, result.Content);
}
