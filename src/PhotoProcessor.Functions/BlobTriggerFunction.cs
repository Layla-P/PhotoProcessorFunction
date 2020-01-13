using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PhotoProcessor.Functions.Services;

namespace PhotoProcessor.Functions
{
    public class BlobTrigger
    {
        private readonly IPhotoService _photoFiddler;

        public BlobTrigger(IPhotoService photoFiddler)
        {
            _photoFiddler = photoFiddler;
        }

        [FunctionName("BlobTrigger")]
        [StorageAccount("AzureWebJobsStorage")]
        public async Task Run([BlobTrigger("images/UploadsImage-{name}")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            // send photo to Pho.to API and update status

            var path = Environment.GetEnvironmentVariable("BlobStoragePath");


            var imagePath = $"{path}images/UploadsImage-{name}";
            log.LogInformation(imagePath);

            var processResponse = await _photoFiddler.Process(imagePath, name);

            log.LogInformation($"Process status: {processResponse.GeneralStatusEnum}, Image URL: {processResponse.ProcessedImageUrl}");

        }
    }
}
