using HikingPal.Models;

namespace HikingPal.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CreateUser(User user);
        Task<bool> DeleteUser(Guid userID);
        Task<List<UserDTO>> GetAllUser();
        Task<User> GetUser(string userName);
        Task<User> Login(string loginName);
        Task<Role> GetUserRole(string userName);
        Task<List<Role>> GetAllUserRole();
    }
}
