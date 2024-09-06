using Microsoft.SemanticKernel;
using SemanticKernel.Plugins.Models;
using System.ComponentModel;
using System.Net.Http.Json;

namespace SemanticKernel.Plugins.Plugins.TicketPlugin
{
    public class TicketPlugin
    {
        [KernelFunction, Description("Get a list of tickets given a specific search query")]
        public async Task<List<Ticket>> GetTicketsAsync([Description("The search query for the ticket")] string searchQuery)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://ticketapi-net8.azurewebsites.net");
            var response = await client.GetFromJsonAsync<List<Ticket>>($"/api/getticket?search={searchQuery}");
            return response;
        }
    }
}