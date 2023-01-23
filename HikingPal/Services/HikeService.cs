using HikingPal.Models;
using HikingPal.Repositories;

namespace HikingPal.Services
{
    public class HikeService : IHikeService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IHikeRepository _hikeRepository;

        public HikeService(ILogger<UserService> logger, IHikeRepository hikeRepository) { 
            _logger = logger;
            _hikeRepository = hikeRepository;
        }

        public async Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff)
        {
            //check the parameter

            bool isSubcribed = await _hikeRepository.SubscribeHike(hikeUser, onOff);
            return isSubcribed;
        }


        public async Task<Hike?> CreateHike(Hike newHike, string userGuid)
        {
            _logger.LogInformation($"Hike created by user.");

            //some check need here....
            newHike.AuthorID = Guid.Parse(userGuid);
            Hike createdHike = await _hikeRepository.CreateHike(newHike);
            if(createdHike!=null)
            {
                HikeUser ownerHiker = new HikeUser()
                {
                    HikeID = createdHike.HikeID,
                    UserID = newHike.AuthorID
                };
                if (await _hikeRepository.SubscribeHike(ownerHiker, true))
                {                    
                    return createdHike;
                }
            }
            return null;
        }

        public async Task<HikeResponse?> GetHike(string hikeID)
        {
            Guid hikeGuid = new Guid(hikeID);

            return await _hikeRepository.GetHike(hikeGuid);
            
        }

        public async Task<List<HikeResponse>> GetHikes()
        {
            var allHikes = await _hikeRepository.GetAllHikes();
            
            return allHikes;
        }

        public async Task<bool> DeleteHike(string hikeGuidString)
        {
            Guid hikeGuid = new Guid(hikeGuidString);

            return await _hikeRepository.DeleteHike(hikeGuid);
        }

        public async Task<bool> ModifyHike(Hike hikeModify)
        {
            var hikeResult = await _hikeRepository.ModifyHike(hikeModify);

            return hikeResult;
        }
    }
}
