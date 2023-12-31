using KernelMemory.Models;
using Microsoft.KernelMemory;

namespace KernelMemory.Services
{
    public class MemoryService : IMemoryService
    {
        private IKernelMemory kernelMemory;

        public MemoryService(IConfiguration configuration)
        {
            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string deploymentChatName = configuration["AzureOpenAI:DeploymentChatName"];
            string deploymentEmbeddingName = configuration["AzureOpenAI:DeploymentEmbeddingName"];
            string endpoint = configuration["AzureOpenAI:Endpoint"];

            string searchApiKey = configuration["AzureSearch:ApiKey"];
            string searchEndpoint = configuration["AzureSearch:Endpoint"];


            var embeddingConfig = new AzureOpenAIConfig
            {
                APIKey = apiKey,
                Deployment = deploymentEmbeddingName,
                Endpoint = endpoint,
                APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey
            };

            var chatConfig = new AzureOpenAIConfig
            {
                APIKey = apiKey,
                Deployment = deploymentChatName,
                Endpoint = endpoint,
                APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey
            };

        var path = Path.Combine($"{Directory.GetCurrentDirectory()}//Memory");

        kernelMemory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(chatConfig)
            .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
            .WithSimpleVectorDb()
            //uncomment the line below to store the vector database on disk
            //.WithSimpleVectorDb(path)
            //uncomment the line below to use Azure AI Search
            //.WithAzureAISearch(searchEndpoint, searchApiKey)
            .Build<MemoryServerless>();
        }

        public async Task<bool> StoreText(string text)
        {
            try
            {
                await kernelMemory.ImportTextAsync(text);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> StoreFile(string path, string filename)
        {
            try
            {
                await kernelMemory.ImportDocumentAsync(path, filename);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> StoreWebsite(string url)
        {
            try
            {
                await kernelMemory.ImportWebPageAsync(url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<KernelResponse> AskQuestion(string question)
        {
            var answer = await kernelMemory.AskAsync(question);
            
            var response = new KernelResponse
            {
                Answer = answer.Result,
                Citations = answer.RelevantSources
            };

            return response;
        }
    }
}
