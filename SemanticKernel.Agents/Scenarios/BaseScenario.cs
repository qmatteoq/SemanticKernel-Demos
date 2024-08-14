using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernel.Agents.Scenarios
{
    public class BaseScenario
    {
        protected AgentGroupChat chat;
        public async Task ExecuteScenario(string prompt)
        {
            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, prompt));
            await foreach (var content in chat.InvokeAsync())
            {
                Console.WriteLine();
                Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
                Console.WriteLine();
            }

            Console.WriteLine($"# IS COMPLETE: {chat.IsComplete}");
        }
    }
}
