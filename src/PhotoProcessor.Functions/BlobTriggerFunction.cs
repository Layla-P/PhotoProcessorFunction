using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PhotoProcessor.Functions
{
    public static class BlobTrigger
    {
        [FunctionName("BlobTrigger")]
        public static void Run([BlobTrigger("images/OriginalImage-{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
