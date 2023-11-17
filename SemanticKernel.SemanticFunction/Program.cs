using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("a5d80f2e-d240-478c-b95f-f14c2979d6c4")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernelBuilder = new KernelBuilder();
kernelBuilder.
    WithAzureOpenAIChatCompletionService(deploymentName, endpoint, apiKey);

var kernel = kernelBuilder.Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

kernel.ImportSemanticFunctionsFromDirectory(pluginsDirectory, "MailPlugin");

var function = kernel.Functions.GetFunction("MailPlugin", "WriteBusinessMail");

ContextVariables variables = new ContextVariables
{
    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
};

var result = await kernel.RunAsync(
    variables,
    function
);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();