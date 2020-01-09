namespace PhotoProcessor.Functions.Models
{
    public class PhotoApiSettings : IPhotoApiSettings
    {
        public string PrivateKey { get; set; }
        public string AppId { get; set; }
    }
}