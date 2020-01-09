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
        private IBlobContext _blobContext;
        private ITableDbContext _tableDbContext;
        private ILogger _log;
        public DataRepository(
            IBlobContext blobContext,
            ITableDbContext tableDbContext,
            ILoggerFactory log)
        {
            _blobContext = blobContext;
            _tableDbContext = tableDbContext;
            _log = log.CreateLogger<DataRepository>();
        }

        public async Task<GeneralStatusEnum> Save(byte[] imageBytes, ProcessStatusEnum status)
        {
            var id = Guid.NewGuid();
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

        public async Task<GeneralStatusEnum> UpdateTable(string id, ProcessStatusEnum status)
        {
            var fileName = $"UploadsImage-{id}.jpg";
            var generalStatusCode = await SaveImageDetails(fileName, id, status);
            return generalStatusCode;
        }

        private async Task<GeneralStatusEnum> SaveImageDetails(string fileName, string id, ProcessStatusEnum status)
        {
            var uploadEntity = new ImageEntity
            {
                PartitionKey = "Uploads",
                RowKey = id.ToString(),
                FileName = fileName,
                ProcessStatusEnum = status.EnumValue()
            };

            return await _tableDbContext.InsertOrMergeEntityAsync(uploadEntity);
        }
    }
}
