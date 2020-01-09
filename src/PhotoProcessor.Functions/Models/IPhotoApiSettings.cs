namespace PhotoProcessor.Functions.Models
{
    public interface IPhotoApiSettings
    {
        string AppId { get; set; }
        string PrivateKey { get; set; }
    }
}