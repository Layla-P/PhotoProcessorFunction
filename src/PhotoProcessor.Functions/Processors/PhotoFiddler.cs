using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using PhotoProcessor.Functions.Models;
using Microsoft.Extensions.Logging;
using PhotoProcessor.Functions.Data;

namespace PhotoProcessor.Functions.Processors
{
    public class PhotoFiddler : IPhotoFiddler
    {
        private readonly IPhotoApiSettings _photoApiSettings;
        private readonly IDataRepository _dataRepository;
        private ILogger _log;
        public PhotoFiddler(IPhotoApiSettings photoApiSettings, ILoggerFactory log, IDataRepository dataRepository)
        {
            _photoApiSettings = photoApiSettings;
            _log = log.CreateLogger<PhotoFiddler>();
            _dataRepository = dataRepository;
        }

        public async Task<string> Process(string incomingImageUrl, string fileName)
        {

            var status = await GetProcessedImage(incomingImageUrl, fileName);

            _log.LogInformation(status);

            return status;
        }

        private async Task<string> GetProcessedImage(string imageUrl, string fileName)
        {
            var processedImageUrl = string.Empty;
            var id = fileName.Split(".")[0];

            string status;
            using (var httpClient = new HttpClient())
            {
                var apiEndPointPost = "http://opeapi.ws.pho.to/addtask";
                var apiEndPointGet = "http://opeapi.ws.pho.to/getresult?request_id=";
                string requestId = "";

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                string xmlMessage = $@"<image_process_call>
                                      <image_url>{imageUrl}</image_url>
                                    <methods_list>
                                        <method>
                                            <name>animated_sparkles</name>
                                            <params>type=10</params>
                                        </method>
                                    </methods_list>
                                </image_process_call>";

                var key = Encoding.ASCII.GetBytes(_photoApiSettings.PrivateKey);
                var keySha = EncodeKey(xmlMessage, key);
                var values = new Dictionary<string, string>
                    {
                        { "app_id", _photoApiSettings.AppId },
                        { "sign_data", keySha },
                        {"data", xmlMessage}
                    };

                var content = new FormUrlEncodedContent(values);

                var responseMessage = await
                    httpClient
                        .PostAsync(apiEndPointPost, content);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    await _dataRepository.UpdateTable(id, ProcessStatusEnum.Failed);
                    return "SomethingWentWrong";
                }

                
                await _dataRepository.UpdateTable(id, ProcessStatusEnum.Processing);

                var response = await responseMessage.Content.ReadAsStringAsync();

                var xml = XElement.Parse(response).Descendants().FirstOrDefault(x => x.Name == "request_id");
                requestId = xml?.Value;



                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    var url = apiEndPointGet + requestId;
                    var responseGet = await httpClient.GetAsync(url);
                    var contentString = await responseGet.Content.ReadAsStringAsync();

                    var xmlGet = XElement.Parse(contentString).Descendants();
                    var xmlStatus = xmlGet.FirstOrDefault(x => x.Name == "status");
                    status = xmlStatus?.Value;
                    ++i;

                    if (status == "BadRequest")
                    {
                        break;
                    }

                    if (status == "OK")
                    {
                        await _dataRepository.UpdateTable(id, ProcessStatusEnum.Completed);

                        var xmlUrl = xmlGet.FirstOrDefault(x => x.Name == "result_url");
                        processedImageUrl = xmlUrl?.Value ?? "empty node";
                    }
                }
                while (i < 10 && status == "InProgress");

                if (i == 10 && status == "InProgress")
                {
                    _log.LogInformation("Retrieve processed image : Timeout error.");
                    return string.Empty;
                }

            }
            _log.LogInformation(processedImageUrl);
            //return processedImageUrl;
            return status;

        }

        private string EncodeKey(string input, byte[] key)
        {
            var encodedKey = new HMACSHA1(key);
            var byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            return encodedKey.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }

      

    }
}

