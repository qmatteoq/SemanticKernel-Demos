using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernel.Basic
{
    public class MyHttpMessageHandler : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                request.RequestUri = new Uri($"http://localhost:1234{request.RequestUri.PathAndQuery}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
