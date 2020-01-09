using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Processors
{
    public interface IPhotoFiddler
    {
        Task<string> Process(string incomingImageUrl);
    }
}