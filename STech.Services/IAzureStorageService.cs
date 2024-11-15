using Azure.Storage.Blobs.Models;
using Azure;

namespace STech.Services
{
    public interface IAzureStorageService
    {
        string GetContainerName();
        string GetBlobUrl();
        Task<string?> UploadImage(string imagePath, byte[] imageBytes);
        Task<bool> DeleteImage(string imageUrl);
        AsyncPageable<BlobItem> GetBlobs(string folderName);
    }
}
