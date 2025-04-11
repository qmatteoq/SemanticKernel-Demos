using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var builder = Kernel.CreateBuilder();

builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
var kernel = builder.Build();

await using var mcpClient = await McpClientFactory.CreateAsync(

    new StdioClientTransport(new () {
        Name = "MyFirstMCP",
        Command = "dotnet",
        Arguments = ["run", "--project", "d:\\src\\samples\\MCPServer\\MCPServer.csproj"],
    })
);

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Tool: {tool.Name} - {tool.Description}");
}

kernel.Plugins.AddFromFunctions("MyFirstMCP", tools.Select(x => x.AsKernelFunction()));

OpenAIPromptExecutionSettings executionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
};

// var prompt = "What's the weather in Seattle?";
// var prompt = "Reverse the following string: 'Hello World!'";
var prompt = "Give me the result of 2 + 2";
var result = await kernel.InvokePromptAsync(prompt, new(executionSettings));
Console.WriteLine($"Result: {result}");