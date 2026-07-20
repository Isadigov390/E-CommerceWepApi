using Shopping.Application.ServiceInterfaces;

namespace Shopping.WebApi.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Task DeleteAsync(string relativePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public async Task<string> SaveAsync(byte[] bytes, string extension)
        {
            var folder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");
            Directory.CreateDirectory(folder); // creates it if missing

            var newName = $"{Guid.NewGuid()}{extension}";       // GUID here
            var fullPath = Path.Combine(folder, newName);

            await File.WriteAllBytesAsync(fullPath, bytes);

            return $"Images/Products/{newName}"; // relative path -> goes to DB
        }
    }
}
