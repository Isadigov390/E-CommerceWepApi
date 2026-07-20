namespace Shopping.Application.ServiceInterfaces
{
    public interface IFileService
    {
        Task<string> SaveAsync(byte[] bytes, string extension);
        Task DeleteAsync(string relativePath);
    }
}
