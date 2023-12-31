
using KernelMemory.Models;

namespace KernelMemory.Services
{
    public interface IMemoryService
    {
        Task<bool> StoreText(string text);

        Task<bool> StoreFile(string content, string filename);

        Task<bool> StoreWebsite(string url);

        Task<KernelResponse> AskQuestion(string question);
    }
}