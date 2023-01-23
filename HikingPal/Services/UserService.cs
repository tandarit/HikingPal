using HikingPal.Models;
using HikingPal.Repositories;
using Microsoft.Extensions.Options;

namespace HikingPal.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly HashingOptions _hashingOptions;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasher, IOptions<HashingOptions> options, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _hashingOptions = options.Value;
            _logger = logger;
        }

        public async Task<User?> CreateUser(UserCreateRequest requestedUser)
        {
            if (requestedUser == null)
            {
                return null;
            }

            //check user name in the database
            var userCheck = await _userRepository.GetUser(requestedUser.UserName);
            if (userCheck != null)
            {
                return null;
            }

            //Default hiker role select
            var roleList = await GetAllUserRole();
            var hikerRole = roleList.FirstOrDefault<Role>(r => r.RoleName=="Hiker");

            User user = new User();

            Random random = new Random();
            int iteration = random.Next(_hashingOptions.IterationsMin, _hashingOptions.IterationsMax);           

            HashObject hashObject = _passwordHasher.Hash(requestedUser.Password, iteration);

            user.UserName = requestedUser.UserName;
            user.Email = requestedUser.Email;
            user.FirstName = requestedUser.FirstName;
            user.LastName = requestedUser.LastName;
            user.Password = hashObject.Key;
            user.Salt = hashObject.Salt;
            user.Iteration = iteration;
            user.Role = hikerRole;        //the basic role is Hiker

            var createResult = await _userRepository.CreateUser(user);

            if(createResult)
            {
                return user;
            }
            return null;
        }

        public async Task<List<User>> ListAllUser()
        {
            return await _userRepository.GetAllUser();
        }

        public async Task<User> GetUser(string userName)
        {
            return await _userRepository.GetUser(userName);
        }

        public async Task<User> Login(LoginUserRequest loginUser)
        {
            var user = await _userRepository.Login(loginUser.UserName);
            if (user == null)
            {
                return null;
            }

            string hash = user.Iteration + "." + user.Salt + "." + user.Password;

            if (_passwordHasher.Check(hash, loginUser.Password))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<Role> GetUserRole(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            Role foundUser = await _userRepository.GetUserRole(userName);

            if (foundUser == null)
            {
                return new Role();
            }

            return foundUser;
        }

        public async Task<List<Role>> GetAllUserRole()
        {            
            var foundAllUser = await _userRepository.GetAllUserRole();            

            return foundAllUser;
        }
    }
}

