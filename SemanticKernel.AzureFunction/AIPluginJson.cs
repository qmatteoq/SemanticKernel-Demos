using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SemanticKernel.AzureFunction
{
    public class AIPluginJson
    {
        [Function("GetAIPluginJson")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/ai-plugin.json")] HttpRequestData req)
        {
            var currentDomain = $"{req.Url.Scheme}://{req.Url.Host}:{req.Url.Port}";
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            var result = File.ReadAllText(binDirectory + "/manifest/ai-plugin.json");
            var json = result.Replace("{url}", currentDomain);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(json);

            return response;
        }
    }
}
