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

        public async Task<GeneralStatusEnum> Save(byte[] imageBytes, string prefix, ProcessStatusEnum status)
        {
            var id = Guid.NewGuid();
            var fileName = $"{prefix}Image-{id}.jpg";

            _log.LogInformation($"imageId: {id}");

            var generalStatusCode = await _blobContext.Write(imageBytes, fileName);

            if (generalStatusCode == GeneralStatusEnum.BadRequest)
            {
                return generalStatusCode;
            }

            generalStatusCode = await SaveImageDetails(fileName, id, prefix, status);

            return generalStatusCode;
        }

        private async Task<GeneralStatusEnum> SaveImageDetails(string fileName, Guid id, string prefix, ProcessStatusEnum status)
        {
            var uploadEntity = new ImageEntity
            {
                PartitionKey = prefix,
                RowKey = id.ToString(),
                FileName = fileName,
                ProcessStatusEnum = status
            };

            return await _tableDbContext.InsertOrMergeEntityAsync(uploadEntity);
        }
    }
}
