using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;

namespace FunctionApp1
{
    public class Function1
    {
        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // var json = JsonSerializer.Deserialize<JsonRequest>(data);

            // Set name to query string or body data
            // var name = json.path;
            string fileName = "image-" + Guid.NewGuid() + ".jpg";
            log.LogInformation(fileName);
            using (MemoryStream stream = new MemoryStream())
            {
                await req.Body.CopyToAsync(stream);
                byte[] imageBytes = stream.ToArray();
                log.LogInformation(imageBytes.Length.ToString());

                var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
                log.LogInformation("storageConnectionString:" + storageConnectionString);

            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
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
                        return new ContentResult { Content = "Please pass a Path in the request body", ContentType = "application/json", StatusCode = 400 };

                }
            }
         }

            return new ContentResult { Content = "", ContentType = "application/json", StatusCode = 200 };
            //return data == null
            //     ? new ContentResult{Content= "Please pass a Path  in the request body", ContentType = "application/json", StatusCode = 400 }
            //: new ContentResult{Content="", ContentType = "application/json", StatusCode = 200};
        }

    }

    public class JsonRequest
    {
        public string path { get; set; }
    }

    
}

