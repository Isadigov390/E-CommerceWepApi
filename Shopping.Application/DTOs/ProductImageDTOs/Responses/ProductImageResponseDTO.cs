namespace Shopping.Application.DTOs.ProductImageDTOs.Responses
{
    public class ProductImageResponseDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int? ProductId { get; set; }
    }
}
