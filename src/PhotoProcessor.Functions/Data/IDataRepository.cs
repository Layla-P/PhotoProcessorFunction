using PhotoProcessor.Functions.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Data
{
    public interface IDataRepository
    {
        Task<UploadResponse> SaveResponse(byte[] imageBytes, ProcessStatusEnum status);

        Task<GeneralStatusEnum> Save(byte[] imageBytes, ProcessStatusEnum status);

        Task<GeneralStatusEnum> UpdateTable(string id, ProcessStatusEnum status, string imageUrl = null);

        Task<bool> CheckTableRecordAvailable(string id);
    }
}