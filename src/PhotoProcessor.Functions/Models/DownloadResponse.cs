using Functions.Models;

namespace PhotoProcessor.Functions.Models
{
    public class DownloadResponse : ResponseBase
    {
        public ImageEntity ImageEntity { get; set; }
    }
}
