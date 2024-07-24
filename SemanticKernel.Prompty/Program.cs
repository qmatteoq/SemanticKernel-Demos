using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets("9aea1645-00e5-48dc-b396-a39b7d6821ca")
    .Build();

string apiKey = configuration["AzureOpenAI:ApiKey"];
string deploymentName = configuration["AzureOpenAI:DeploymentName"];
string endpoint = configuration["AzureOpenAI:Endpoint"];

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

KernelArguments kernelArguments = new()
    {
        { "question", "Which type of services does Contoso Electronics provide?" },
        { "firstName", "Matteo" },
        { "context", @"Contoso Electronics is a leader in the aerospace industry, providing advanced electronic components for both commercial and military aircraft. 
                    We specialize in creating cutting edge systems that are both reliable and efficient. Our mission is to provide the highest quality aircraft components to our customers, 
                    while maintaining a commitment to safety and excellence. We are proud to have built a strong reputation in the aerospace industry and strive to continually 
                    improve our products and services. Our experienced team of engineers and technicians are dedicated to providing the best products and services to our customers. 
                    With our commitment to excellence, we are sure to remain a leader in the aerospace industry for years to come" }
    };

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var prompty = kernel.CreateFunctionFromPromptyFile("basic.prompty");
#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var result = await prompty.InvokeAsync<string>(kernel, kernelArguments);
Console.WriteLine(result);
