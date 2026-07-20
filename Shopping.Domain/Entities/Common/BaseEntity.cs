using Shopping.Domain.Entities.Interfaces;

namespace Shopping.Domain.Models.Common
{
    public class BaseEntity : AuditableEntity
    {
        public int Id { get; set; }
    }
}
