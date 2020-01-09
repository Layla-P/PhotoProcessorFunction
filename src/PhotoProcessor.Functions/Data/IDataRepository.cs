using PhotoProcessor.Functions.Models;
using System.IO;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Data
{
    public interface IDataRepository
    {
        Task<GeneralStatusEnum> Save(byte[] imageBytes, string prefix, ProcessStatusEnum status);
    }
}