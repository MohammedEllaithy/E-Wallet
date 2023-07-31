using WebApi.Entities;

namespace WebApi.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phoneNumber);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AddUserAsync(User user);
        Task UpdateUser(User user);
    }
}
