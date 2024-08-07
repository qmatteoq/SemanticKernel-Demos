using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Agents
{
    public static class KernelCreator
    {
        public static Kernel CreateKernel()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string deploymentName = configuration["AzureOpenAI:DeploymentName"];
            string endpoint = configuration["AzureOpenAI:Endpoint"];

            var kernel = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
                .Build();


            return kernel;
        }
    }
}
