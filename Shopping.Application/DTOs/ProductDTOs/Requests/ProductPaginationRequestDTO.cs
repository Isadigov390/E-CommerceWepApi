using Shopping.Domain.Enums;

namespace Shopping.Application.DTOs.ProductDTOs.Requests
{
    public class ProductPaginationRequestDTO
    {
        public int Limit { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public string? Search { get; set; }
        public ProductSortBy SortBy { get; set; } = ProductSortBy.Newest;
        public List<int> CategoryIds { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyAvailable { get; set; }
    }
}
