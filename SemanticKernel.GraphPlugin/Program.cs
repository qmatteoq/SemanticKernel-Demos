using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.MsGraph;
using Microsoft.SemanticKernel.Plugins.MsGraph.Connectors;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("ee2ccf8c-37a6-4671-aaf4-a55a16903918")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();



// The client credentials flow requires that you request the
// /.default scope, and pre-configure your permissions on the
// app registration in Azure. An administrator must grant consent
// to those permissions beforehand.
var scopes = new[] { "Calendars.Read" };

// Multi-tenant apps can use "common",
// single-tenant apps must use the tenant ID from the Azure portal
var tenantId = "15b5c0a0-bb97-43e6-807a-d0daf5bb7671";

// Values from app registration
var clientId = "7b105956-e771-4413-8da6-073d6ff01e02";
var clientSecret = "FuI8Q~6AlvsfsFSJ1X~GuF8zND1PCDW8hPE6HdwJ";
var options = new DeviceCodeCredentialOptions
{
    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    ClientId = clientId,
    TenantId = tenantId,
    // Callback function that receives the user prompt
    // Prompt contains the generated device code that user must
    // enter during the auth process in the browser
    DeviceCodeCallback = (code, cancellation) =>
    {
        Console.WriteLine(code.Message);
        return Task.FromResult(0);
    },
};

// https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
var deviceCodeCredential = new DeviceCodeCredential(options);

var graphClient = new GraphServiceClient(deviceCodeCredential, scopes);

#pragma warning disable SKEXP0053 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

OutlookCalendarConnector connector = new OutlookCalendarConnector(graphClient);
CalendarPlugin plugin = new CalendarPlugin(connector);

kernel.ImportPluginFromObject(plugin, "CalendarPlugin");

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions
};

string prompt = "What is my next meeting?";
var results = kernel.InvokePromptStreamingAsync(prompt, new KernelArguments(settings));
await foreach (var message in results)
{
    Console.Write(message);
}

#pragma warning restore SKEXP0053 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


