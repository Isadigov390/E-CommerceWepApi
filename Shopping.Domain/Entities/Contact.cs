using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class Contact : BaseEntity
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}
