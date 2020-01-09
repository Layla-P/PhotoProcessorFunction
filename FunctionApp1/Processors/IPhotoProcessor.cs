using System.Threading.Tasks;

namespace FunctionApp1.Processors
{
    public interface IPhotoProcessor
    {
        Task<string> Process(string incomingImageUrl, string id, string host);
    }
}