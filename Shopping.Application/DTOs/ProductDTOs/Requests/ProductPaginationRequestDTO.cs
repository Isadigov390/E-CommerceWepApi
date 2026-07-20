namespace Shopping.Application.DTOs.ProductDTOs.Requests
{
    public class ProductPaginationRequestDTO
    {
        public int Limit { get; set; } = 10;
        public int Skip { get; set; } = 0;

    }
}
