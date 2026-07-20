using Shopping.Domain.Models.Common;

namespace Shopping.Domain.Models
{
    public class Person : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public User? User { get; set; }
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
