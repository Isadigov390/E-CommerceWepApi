namespace Shopping.Application.DTOs.ProductDetailDTOs
{
    public class ProductDetailRequestDTO
    {
        public string SKU { get; set; } = string.Empty;
        public int Discount { get; set; }
        
        public int Warranty { get; set; }
        public int ProductId { get; set; }
    }
}
