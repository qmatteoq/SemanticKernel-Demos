using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("8a4821ee-3680-41af-8b37-1b9a978ac962")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernelBuilder = new KernelBuilder().
    WithAzureChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

string prompt = """
Rewrite the text between triple backticks into a business mail. Use a professional tone, be clar and concise.
Sign the mail as AI Assistant.

Text: ```{{$input}}```
""";

var mailFunction = kernel.CreateSemanticFunction(prompt);

ContextVariables variables = new ContextVariables
{
    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
};

var output = await kernel.RunAsync(
    variables,
    mailFunction);

Console.WriteLine(output.GetValue<string>());
Console.ReadLine();