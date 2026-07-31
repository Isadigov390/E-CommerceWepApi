using Shopping.Domain.Entities.Accounts;
using Shopping.Domain.Entities.Interfaces;

namespace Shopping.Domain.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);

    }
}
