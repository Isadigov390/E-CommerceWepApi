namespace Shopping.Application.DTOs.ProductDTOs.Responses
{
    public class ProductPagedResponseDTO
    {
        public List<ProductResponseDTO> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
