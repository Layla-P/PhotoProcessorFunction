using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotoProcessor.Functions.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoProcessor.Functions.Data
{
    public class BlobContext : IBlobContext
    {
        private ILogger _log;
        public BlobContext(ILoggerFactory log)
        {
            _log = log.CreateLogger<TableDbContext>();
        }

        public async Task<GeneralStatusEnum> Write(MemoryStream stream, string fileName)
        {
            using (MemoryStream _stream = stream)
            {
                byte[] imageBytes = _stream.ToArray();
                _log.LogInformation(imageBytes.Length.ToString());

                var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
                _log.LogInformation("storageConnectionString:" + storageConnectionString);

                if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();


                    var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(blobContainerName);
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = "image/jpeg";

                    try
                    {
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);
                    }
                    catch (StorageException ex)
                    {
                        if (ex.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.Conflict)
                            return GeneralStatusEnum.BadRequest;
                    }
                }

                return GeneralStatusEnum.Ok;
            }
        }
    }
}
