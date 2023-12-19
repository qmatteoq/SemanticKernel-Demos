using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("8a4821ee-3680-41af-8b37-1b9a978ac962")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

string prompt = """
Rewrite the text between triple backticks into a business mail. Use a professional tone, be clear and concise.
Sign the mail as AI Assistant.

Text: ```{{$input}}```
""";

var mailFunction = kernel.CreateFunctionFromPrompt(prompt, new OpenAIPromptExecutionSettings
{
    Temperature = 0.7,
    MaxTokens = 1000,
});

KernelArguments arguments = new KernelArguments
{
    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
};


//standard function invocation
var response = await kernel.InvokeAsync(mailFunction, arguments);
Console.WriteLine(response.GetValue<string>());

//prompt function invocation
//var response = await kernel.InvokePromptAsync(prompt, arguments: new() {
//    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
//});
//Console.WriteLine(response.GetValue<string>());

//prompt streaming invocation
//var response = kernel.InvokePromptStreamingAsync(prompt, arguments: new()
//{
//    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
//});

//await foreach (var message in response)
//{
//    Console.Write(message);
//}

Console.ReadLine();