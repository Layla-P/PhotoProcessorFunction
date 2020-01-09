using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoProcessor.Functions.Data;
using PhotoProcessor.Functions.Models;

namespace PhotoProcessor.Functions
{
    public class UploadFunction
    {
        private readonly IDataRepository _dataRepository;

        public UploadFunction(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [FunctionName("Upload")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            GeneralStatusEnum status;
            using (MemoryStream stream = new MemoryStream())
            {
                await req.Body.CopyToAsync(stream);
                byte[] imageBytes = stream.ToArray();
                log.LogInformation(imageBytes.Length.ToString());

                status = await _dataRepository.Save(imageBytes, ProcessStatusEnum.Uploaded);

                log.LogInformation($"status:{status}");
            }
            return status != GeneralStatusEnum.Ok
                ? new BadRequestObjectResult("Something went wrong")
                : (ActionResult)new OkObjectResult($"It worked");
        }
    }
}
