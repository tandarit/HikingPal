using HikingPal.Models;
using System.Security.Claims;

namespace HikingPal.Services
{
    public interface IUserService
    {
        Task<UserDTO> CreateUser(UserCreateRequest user);
        Task<bool> DeleteUser(string userID, Claim[] myClaim);
        Task<List<UserDTO>> ListAllUser();
        Task<User> GetUser(string userName);
        Task<User> Login(LoginUserRequest loginUserRequest);
        Task<Role> GetUserRole(string userName);
        Task<List<Role>> GetAllUserRole();
    }
}
