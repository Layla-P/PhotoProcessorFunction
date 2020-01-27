using Microsoft.Extensions.Logging;
using PhotoProcessor.Functions.Data;
using PhotoProcessor.Functions.Models;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly ITableDbContext _tableDbContext;
        private ILogger _log;

        public DownloadService(ILoggerFactory log, ITableDbContext tableDbContext)
        {
            _log = log.CreateLogger<DownloadService>();
            _tableDbContext = tableDbContext;
        }

        public async Task<DownloadResponse> FetchDownload(string id)
        {
            var downloadResponse = new DownloadResponse();

            var imageEntity = await _tableDbContext.GetEntityAsync("Uploads", id);

            // use try/parse instead?
            if (imageEntity.ProcessStatusEnum != ProcessStatusEnum.Completed.EnumValue())
            {
                _log.LogInformation(imageEntity.ProcessStatusEnum);
                downloadResponse.GeneralStatusEnum = GeneralStatusEnum.Processing;
                downloadResponse.ImageEntity = null;
            }
            else
            {
                downloadResponse.GeneralStatusEnum = GeneralStatusEnum.Ok;
                downloadResponse.ImageEntity = imageEntity;
            }

            return downloadResponse;
        }
    }
}
