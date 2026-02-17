using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;

namespace MyAcademyCQRS.Services;

public class GoogleCloudStorageService : ICloudStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public GoogleCloudStorageService(IConfiguration configuration, IWebHostEnvironment env)
    {
        var credentialFileName = configuration["GoogleCloudStorage:CredentialFile"];
        _bucketName = configuration["GoogleCloudStorage:BucketName"] ?? "bakery-images";

        var credentialPath = Path.Combine(env.ContentRootPath, credentialFileName);

        if (!string.IsNullOrEmpty(credentialFileName) && File.Exists(credentialPath))
        {
            var credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);
        }
        else
        {
            // Application Default Credentials (ADC) kullanır
            _storageClient = StorageClient.Create();
        }
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueName = $"{Guid.NewGuid()}-{fileName}";

        await _storageClient.UploadObjectAsync(
            _bucketName,
            uniqueName,
            contentType,
            fileStream);

        return GetFileUrl(uniqueName);
    }

    public async Task DeleteAsync(string fileUrl)
    {
        var objectName = ExtractObjectName(fileUrl);
        if (string.IsNullOrEmpty(objectName)) return;

        try
        {
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Dosya zaten silinmiş — hata verme
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"https://storage.googleapis.com/{_bucketName}/{fileName}";
    }

    private string ExtractObjectName(string fileUrl)
    {
        // URL Format: https://storage.googleapis.com/{bucket}/{object-name}
        var prefix = $"https://storage.googleapis.com/{_bucketName}/";
        if (fileUrl.StartsWith(prefix))
            return fileUrl[prefix.Length..];

        // Alternatif format: https://storage.cloud.google.com/{bucket}/{object-name}
        var altPrefix = $"https://storage.cloud.google.com/{_bucketName}/";
        if (fileUrl.StartsWith(altPrefix))
            return fileUrl[altPrefix.Length..];

        // Son çare — URI'den son segmenti al
        var uri = new Uri(fileUrl);
        return uri.Segments.Last();
    }
}
