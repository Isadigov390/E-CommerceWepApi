using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class ProductImage : BaseEntity
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
