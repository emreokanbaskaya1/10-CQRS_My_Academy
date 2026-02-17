namespace MyAcademyCQRS.Services;

public interface ICloudStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string fileUrl);
    string GetFileUrl(string fileName);
}
