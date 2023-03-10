using HikingPal.Models;

namespace HikingPal.Repositories
{
    public interface IHikeRepository
    {
        Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff);
        Task<HikeDTO?> CreateHike(Hike hike);
        Task<List<HikeDTO>> GetAllHikes();
        Task<HikeDTO> GetHike(Guid hikeID);
        Task<bool> DeleteHike(Guid hikeID);
        Task<bool> ModifyHike(Hike modifiedHike);
    }
}
