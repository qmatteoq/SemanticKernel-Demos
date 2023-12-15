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


var output = await kernel.InvokeAsync(mailFunction, arguments);

//var output = await kernel.InvokePromptAsync(prompt, arguments: new() {
//    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
//});
//var output = kernel.InvokePromptStreamingAsync(prompt, arguments: new()
//{
//    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
//});

//await foreach (var item in output)
//{
//    Console.Write(item.InnerContent);
//}

Console.WriteLine(output.GetValue<string>());
Console.ReadLine();