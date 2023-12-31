using Microsoft.KernelMemory;

namespace KernelMemory.Models
{
    public class KernelResponse
    {
        public string Answer { get; set; }
        public List<Citation> Citations { get; set; }
    }
}
