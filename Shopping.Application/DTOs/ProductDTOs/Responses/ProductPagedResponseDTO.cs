namespace Shopping.Application.DTOs.ProductDTOs.Responses
{
    public class ProductPagedResponseDTO
    {
        public List<ProductWithCategoryResponseDTO> Products { get; set; } = new();
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
        public decimal MaxPrice { get; set; }   
    }
}
