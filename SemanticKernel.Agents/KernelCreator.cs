using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Agents
{
    public static class KernelCreator
    {
        public static Kernel CreateKernel(bool useAzureOpenAI)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string deploymentName = configuration["AzureOpenAI:DeploymentName"];
            string endpoint = configuration["AzureOpenAI:Endpoint"];

            string openAIKey = configuration["OpenAI:ApiKey"];
            string openAIModel = configuration["OpenAI:Model"];

            Kernel? kernel;

            if (useAzureOpenAI)
            {
                kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)                
                .Build();
            }
            else
            {
                kernel = Kernel.CreateBuilder()
                .AddOpenAIChatCompletion(openAIModel, openAIKey)
                .Build();
            }


            return kernel;
        }
    }
}
