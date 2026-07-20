using Microsoft.Extensions.Configuration;
using Shopping.Application.ServiceInterfaces;

namespace Shopping.Application.Services
{
    public class UrlService : IUrlService
    {
        private readonly IConfiguration _configuration;
        public UrlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BuildUrl(string relativePath)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"]!.TrimEnd('/');
            return $"{baseUrl}/{relativePath.TrimStart('/')}";
        }
    }

}
