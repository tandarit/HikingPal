using HikingPal.DataContext;
using HikingPal.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HikingPal.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly HikingPalDataContext _context;

        public UserRepository(ILogger<UserRepository> logger, HikingPalDataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> CreateUser(User user)
        {
            _logger.LogInformation("Repository of creation user function!");

            _context.Users.Add(user);
            var effectedRows = await _context.SaveChangesAsync();

            return effectedRows > 0;
        }

        public async Task<List<User>> GetAllUser()
        {
            _logger.LogInformation("Repository of all user list function!");
            var userList = from user in _context.Users 
                           join role in _context.Roles on user.RoleID equals role.RoleID
                           select new User()
                           {
                               UserID= user.UserID,
                               RoleID=role.RoleID,
                               Email=user.Email,
                               FirstName=user.FirstName,
                               Iteration=user.Iteration,
                               LastName=user.LastName,
                               UserName=user.UserName,
                               Salt=user.Salt,
                               Password=user.Password,
                               Role = new Role() { 
                                    RoleID = role.RoleID,
                                    RoleName = role.RoleName,
                                    RoleDescription = role.RoleDescription
                               }
                           };
            //var userList = await _context.Users.Where<User>(r=>r.Role.RoleID==) .ToListAsync<User>();
            return await userList.ToListAsync<User>();
        }

        public async Task<User> GetUser(string userName)
        {
            _logger.LogInformation("Repository of user info function!");
            var user = await _context.Users.FirstOrDefaultAsync<User>(u=>u.UserName == userName);
            return user;
        }

        public async Task<Role> GetUserRole(string userName)
        {
            _logger.LogInformation("Get user role!");
            User foundedUser = await _context.Users.FirstOrDefaultAsync<User>(q => q.UserName == userName);

            if (foundedUser == null)
            {
                return new Role();
            }

            var foundedRole = await _context.Roles.FirstOrDefaultAsync<Role>(r=> r.RoleID == foundedUser.RoleID);
            return foundedRole;
        }

        public async Task<List<Role>> GetAllUserRole()
        {
            _logger.LogInformation("Get all user role!");
            
            var foundedRoles = _context.Roles.ToList<Role>();
            return foundedRoles;
        }

        public async Task<User> Login(string loginName)
        {
            var userLogin = from user in _context.Users
                            join role in _context.Roles on user.RoleID equals role.RoleID
                            where user.UserName == loginName
                            select new User()
                             {
                                UserID = user.UserID,
                                RoleID = role.RoleID,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                Iteration = user.Iteration,
                                LastName = user.LastName,
                                UserName = user.UserName,
                                Salt = user.Salt,
                                Password = user.Password,
                                Role = new Role()
                                {
                                                RoleID = role.RoleID,
                                                RoleName = role.RoleName,
                                                RoleDescription = role.RoleDescription
                                }
                             };
            return await userLogin.FirstOrDefaultAsync<User>();

        }
    }
}
