using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("5b68dc4b-5ae4-44c4-a65b-6ae334716c74")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];
string bingKey  = configuration["Bing:ApiKey"];

var kernel = new KernelBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, modelId, endpoint, apiKey)
    .Build();


#pragma warning disable SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var bingConnector = new BingConnector(bingKey);
var plugin = new WebSearchEnginePlugin(bingConnector);
//kernel.ImportPluginFromObject(plugin, "BingPlugin");

OpenAIPromptExecutionSettings settings = new()
{
    FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
};

var results = kernel.InvokePromptStreamingAsync("Which is the latest version of Semantic Kernel from Microsoft?", new KernelArguments(settings));
await foreach (var result in results)
{
    Console.Write(result.InnerContent);
}   

Console.WriteLine();
Console.ReadLine();

#pragma warning restore SKEXP0054 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.