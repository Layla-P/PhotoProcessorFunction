using PhotoProcessor.Functions.Models;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Services
{
    public interface IDownloadService
    {
        Task<DownloadResponse> FetchDownload(string id);
    }
}