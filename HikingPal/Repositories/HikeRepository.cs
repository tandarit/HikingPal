using HikingPal.DataContext;
using HikingPal.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HikingPal.Repositories
{
    public class HikeRepository : IHikeRepository
    {
        private readonly ILogger<HikeRepository> _logger;
        private readonly HikingPalDataContext _context;

        public HikeRepository(ILogger<HikeRepository> logger, HikingPalDataContext context) {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> DeleteHike(Guid hikeID)
        {
            _logger.LogInformation($"Delete hike, ID: {hikeID.ToString()}.");

            var deletehike = _context.Hikes.Where<Hike>(h => h.HikeID == hikeID).SingleOrDefault<Hike>();
            var deleteHikeUserArray = _context.HikeUsers.Where(hu => hu.HikeID == hikeID).ToList();

            _context.Hikes.Remove(deletehike);
            _context.HikeUsers.RemoveRange(deleteHikeUserArray);

            var result = await _context.SaveChangesAsync();

            return result>0;
        }

        public async Task<bool> ModifyHike(Hike modifiedHike)
        {
            //the Author can not changed... and hiker subscibe not working....
            var oldModifiedHike = await _context.Hikes.FirstAsync<Hike>(h=>h.HikeID == modifiedHike.HikeID);
            if(oldModifiedHike == null)
            {
                return false;
            }

            oldModifiedHike.Name = modifiedHike.Name;
            oldModifiedHike.PhotoUrl = modifiedHike.PhotoUrl;
            oldModifiedHike.Description = modifiedHike.Description;
            oldModifiedHike.PhotoTitle = modifiedHike.PhotoTitle;

            _context.Hikes.Update(oldModifiedHike);

            var result = await _context.SaveChangesAsync();

            if (result == 0) return false;

            return true;
        }

        public async Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff)
        {
            if (onOff)
            {
                _logger.LogInformation($"Subscribe user {hikeUser.UserID.ToString()} to the hike {hikeUser.HikeID.ToString()}!");
                var resultEntity = await _context.HikeUsers.AddAsync(hikeUser);                
            }
            else
            {
                _logger.LogInformation($"UnSubscribe user {hikeUser.UserID.ToString()} to the hike {hikeUser.HikeID.ToString()}!");
                var removeSubscibedHikeArray = (from removehikeuser in _context.HikeUsers
                                               where removehikeuser.UserID == hikeUser.UserID && removehikeuser.HikeID == hikeUser.HikeID
                                               select removehikeuser).ToArray<HikeUser>();

                if(removeSubscibedHikeArray.Length == 0)
                    return false;

                _context.HikeUsers.RemoveRange(removeSubscibedHikeArray);
            }

            var result = await _context.SaveChangesAsync();
            return result>0;
        }

        

        public async Task<HikeDTO?> CreateHike(Hike hike)
        {
            _logger.LogInformation("Repository of creation hike function!");            

            var result = await _context.Hikes.AddAsync(hike);

            var effectedRows = await _context.SaveChangesAsync();
            if(effectedRows==0)
            {
                return null;
            }
            return result.Entity;
        }

       

        public async Task<List<HikeDTO>> GetAllHikes()
        {
            _logger.LogInformation("Repository of query of all hike!");

            var getAllHikeList = await (from hike in _context.Hikes
                                  join user in _context.Users on hike.AuthorID equals user.UserID                                                                     
                                  select new HikeDTO()
                                  {
                                      HikeID=hike.HikeID,
                                      AuthorDTO= new UserDTO() {
                                          UserID = user.UserID,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          Email = user.Email,
                                          Role = user.Role
                                      },                                                                          
                                      Description=hike.Description,
                                      Name=hike.Name,
                                      PhotoTitle=hike.PhotoTitle,
                                      PhotoUrl=hike.PhotoUrl,
                                      HikersDTO = (from hikeuser in _context.HikeUsers
                                                    join userLocal in _context.Users on hikeuser.UserID equals userLocal.UserID
                                                    where hikeuser.HikeID == hike.HikeID
                                                    select new UserDTO()
                                                    {
                                                        UserID = userLocal.UserID,
                                                        Email = userLocal.Email,
                                                        FirstName = userLocal.FirstName,
                                                        LastName = userLocal.LastName,
                                                        Role = userLocal.Role
                                                    }).ToList<UserDTO>()
                                  }).ToListAsync<HikeDTO>();
              return getAllHikeList;
            
        }

        public async Task<HikeDTO?> GetHike(Guid hikeID)
        {
            var getHikeQuery = from hike in _context.Hikes
                               join user in _context.Users on hike.AuthorID equals user.UserID
                               where hike.HikeID == hikeID
                               select new HikeDTO()
                               {
                                   HikeID = hike.HikeID,
                                   AuthorDTO = new UserDTO()
                                   {
                                       UserID = user.UserID,
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       Email = user.Email,
                                       Role = user.Role
                                   },
                                   Description = hike.Description,
                                   Name = hike.Name,
                                   PhotoTitle = hike.PhotoTitle,
                                   PhotoUrl = hike.PhotoUrl,
                                   HikersDTO = (from hikeuser in _context.HikeUsers
                                             join userLocal in _context.Users on hikeuser.UserID equals userLocal.UserID
                                             where hikeuser.HikeID == hike.HikeID
                                             select new UserDTO()
                                             {
                                                 UserID = userLocal.UserID,
                                                 Email = userLocal.Email,
                                                 FirstName = userLocal.FirstName,
                                                 LastName = userLocal.LastName,
                                                 Role = userLocal.Role
                                             }).ToList<UserDTO>()
                               };

            return await getHikeQuery.FirstOrDefaultAsync<HikeDTO>();
        }

        

        
    }
}
