using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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
    .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
    .WithAzureAISearchMemoryDb(searchEndpoint, searchApiKey)
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentChatName, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
kernel.ImportPluginFromPromptDirectory(pluginsDirectory + "\\MailPlugin", "MailPlugin");

var plugin = new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true);
kernel.ImportPluginFromObject(plugin, "memory");

// RAG combined with plugins

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

//var prompt = @"
//            Question to Kernel Memory: {{$input}}

//            Kernel Memory Answer: {{memory.ask $input}}

//            If the answer is empty say 'I don't know', otherwise reply with a business mail to share the answer.
//            ";


//KernelArguments arguments = new KernelArguments(settings)
//{
//    { "input", "What is Contoso Electronics?" },
//};

//var response = await kernel.InvokePromptAsync(prompt, arguments);

//Console.WriteLine(response.GetValue<string>());
//Console.ReadLine();

//chat experience

var chatHistory = new ChatHistory();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    var message = Console.ReadLine();

    var prompt = $@"
            The following message contains a question and, optionally, a task to perform on the answer. 
            First ask the question to Kernel Memory, then perform the requested task.
            If Kernel Memory doesn't know the answer, say 'I don't know'.
            If the question is about a topic that isn't convered by the information in Kernel Memory, say 'I can't answer that'.

            Question: {message}

            ";


    chatHistory.AddMessage(AuthorRole.User, prompt);
    var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, settings, kernel);
    Console.WriteLine(result.Content);
    chatHistory.AddMessage(AuthorRole.Assistant, result.Content);
}
