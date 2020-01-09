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
using Microsoft.Extensions.Options;

using FunctionApp1.Models;

namespace FunctionApp1.Processors
{
     public class PhotoProcessor : IPhotoProcessor
    { private readonly PhotoApiSettings _photoApiSettings;
        public PhotoProcessor(IOptions<PhotoApiSettings> photoApiSettings)
        {
            _photoApiSettings = photoApiSettings.Value;
        }
        public async Task<string> Process(string incomingImageUrl, string id, string host)
        {
            var imageLocation = await SaveImageLocallytest(incomingImageUrl, id);

            var imageUrl = host + imageLocation;

            var processedImageUrl = await GetProcessedImage(imageUrl);

            Console.WriteLine(processedImageUrl);

            return processedImageUrl;
        }

        private async Task<string> GetProcessedImage(string imageUrl)
        {
            string processedImageUrl = string.Empty;

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
                                            <name>caricature</name>
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

                if (responseMessage.IsSuccessStatusCode)
                {
                    var response = await responseMessage.Content.ReadAsStringAsync();

                    var xml = XElement.Parse(response).Descendants().FirstOrDefault(x => x.Name == "request_id");
                    requestId = xml?.Value;
                }


                string status;

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

                    if (status == "OK")
                    {
                        var xmlUrl = xmlGet.FirstOrDefault(x => x.Name == "result_url");
                        processedImageUrl = xmlUrl?.Value ?? "empty node";
                    }
                }
                while (i < 10 && status == "InProgress");

                if (i == 10 && status == "InProgress")
                {
                    Console.WriteLine("Retrieve processed image : Timeout error.");
                    return string.Empty;
                }

            }

            return processedImageUrl;

        }

        private string EncodeKey(string input, byte[] key)
        {
            var encodedKey = new HMACSHA1(key);
            var byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);
            return encodedKey.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }

        private async Task<string> SaveImageLocallytest(string imageUrl, string sid)
        {
            var root = "/wwwroot";
            var dir = "/images/";
            var filename = $"{sid}.jpg";
            var path = Environment.CurrentDirectory + root + dir;
            var saveLocation = path + filename;
            var rootLocation = dir + filename;

            if (!Directory.Exists(path))
            {
                var di = Directory.CreateDirectory(path);
            }

            using (var httpClient = new HttpClient())
            {
                byte[] imageBytes = await
                    httpClient
                        .GetByteArrayAsync(imageUrl);
                FileStream fs = new FileStream(saveLocation, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(imageBytes);
            }

            return rootLocation;
        }


    }
}

