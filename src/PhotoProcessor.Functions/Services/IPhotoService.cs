using PhotoProcessor.Functions.Models;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Services
{
    public interface IPhotoService
    {
        Task<ProcessResponse> Process(string incomingImageUrl, string fileName);
    }
}