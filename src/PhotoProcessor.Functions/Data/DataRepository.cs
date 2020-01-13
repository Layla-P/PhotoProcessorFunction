using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Functions.Models;
using Microsoft.Extensions.Logging;
using PhotoProcessor.Functions.Models;

namespace PhotoProcessor.Functions.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly IBlobContext _blobContext;
        private readonly ITableDbContext _tableDbContext;
        private readonly ILogger _log;
        private Guid id;
        public DataRepository(
            IBlobContext blobContext,
            ITableDbContext tableDbContext,
            ILoggerFactory log)
        {
            _blobContext = blobContext;
            _tableDbContext = tableDbContext;
            _log = log.CreateLogger<DataRepository>();
        }

        public async Task<UploadResponse> SaveResponse(byte[] imageBytes, ProcessStatusEnum status)
        {
            var uploadResponse = new UploadResponse();
            uploadResponse.GeneralStatusEnum = await Save(imageBytes, status);
            uploadResponse.Id = id.ToString();
            return uploadResponse;
        }

        public async Task<GeneralStatusEnum> Save(byte[] imageBytes, ProcessStatusEnum status)
        {
            SetId();
            var fileName = $"UploadsImage-{id}.jpg";

            _log.LogInformation($"imageId: {id}");

            var generalStatusCode = await _blobContext.Write(imageBytes, fileName);

            if (generalStatusCode == GeneralStatusEnum.BadRequest)
            {
                return generalStatusCode;
            }

            generalStatusCode = await SaveImageDetails(fileName, id.ToString(), status);

            return generalStatusCode;
        }

        public async Task<GeneralStatusEnum> UpdateTable(string id, ProcessStatusEnum status, string imageUrl = null)
        {
            var fileName = $"UploadsImage-{id}.jpg";
            var generalStatusCode = await SaveImageDetails(fileName, id, status, imageUrl);
            return generalStatusCode;
        }

        private async Task<GeneralStatusEnum> SaveImageDetails(
            string fileName,
            string id,
            ProcessStatusEnum status,
            string imageUrl = null)
        {
            var uploadEntity = new ImageEntity("Uploads",id, fileName, status, imageUrl);
            return await _tableDbContext.InsertOrMergeEntityAsync(uploadEntity);
        }

        public async Task<bool> CheckTableRecordAvailable(string id)
        {

            var record = await _tableDbContext.GetEntityAsync("Uploads", id);

            return record == null ? false : true;

        }

        private void SetId()
        {
            id = Guid.NewGuid();
        }
    }
}
