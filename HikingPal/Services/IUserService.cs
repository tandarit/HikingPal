using HikingPal.Models;

namespace HikingPal.Services
{
    public interface IUserService
    {
        Task<User?> CreateUser(UserCreateRequest user);
        Task<List<User>> ListAllUser();
        Task<User> GetUser(string userName);
        Task<User> Login(LoginUserRequest loginUserRequest);
        Task<Role> GetUserRole(string userName);
        Task<List<Role>> GetAllUserRole();
    }
}
