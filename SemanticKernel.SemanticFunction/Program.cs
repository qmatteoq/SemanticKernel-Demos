﻿using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("a5d80f2e-d240-478c-b95f-f14c2979d6c4")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");

//use classic approach

kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "MailPlugin");
var function = kernel.Plugins.GetFunction("MailPlugin", "WriteBusinessMail");

//use yaml approach

//var writeMailYaml = File.ReadAllText($"{pluginsDirectory}\\MailPluginYaml\\WriteBusinessMail.yaml");
//var function = kernel.CreateFunctionFromPromptYaml(writeMailYaml);
//var plugin = KernelPluginFactory.CreateFromFunctions("MailPlugin", new[] { function });
//kernel.Plugins.Add(plugin);

KernelArguments variables = new KernelArguments()
{
    { "input", "Tell David that I'm going to finish the business plan by the end of the week." }
};

var result = await kernel.InvokeAsync(function, variables);

Console.WriteLine(result.GetValue<string>());
Console.ReadLine();