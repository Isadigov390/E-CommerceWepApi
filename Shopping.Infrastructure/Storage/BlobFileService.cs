using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.Infrastructure.Storage
{
    public class BlobFileService : IFileService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobFileService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlob:ConnectionString"] ?? throw new InvalidOperationException("AzureBlob:ConnectionString is missing.");
            _containerName = configuration["AzureBlob:ContainerName"] ?? throw new InvalidOperationException("AzureBlob:ContainerName is missing.");
        }

        public async Task<string> SaveAsync(byte[] bytes, string extension)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobName = $"{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(blobName);
            var headers = new BlobHttpHeaders { ContentType = GetContentType(extension) };

            using var stream = new MemoryStream(bytes);

            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers });

            return blobName;
        }

        public async Task DeleteAsync(string blobName)
        {
            var containerClient = new BlobContainerClient(_connectionString, _containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}