using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("a5d80f2e-d240-478c-b95f-f14c2979d6c4")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];
string modelId = configuration["AzureOpenAI:ModelId"];

var kernel = new KernelBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, modelId, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

//kernel.ImportPluginFromPromptDirectory(pluginsDirectory + "\\MailPlugin", "MailPlugin");
var writeMailYaml = File.ReadAllText($"{pluginsDirectory}\\MailPluginYaml\\WriteBusinessMail.yaml");
var function = kernel.CreateFunctionFromPromptYaml(writeMailYaml);

OpenAIPromptExecutionSettings settings = new()
{
    FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
};


KernelArguments variables = new KernelArguments(settings)
{
    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
};

//var result = await kernel.InvokeAsync(function, variables);
var result = await kernel.InvokePromptAsync("Write a business mail about the following topic: {{$input}}", variables);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();