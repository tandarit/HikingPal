using HikingPal.Models;

namespace HikingPal.Repositories
{
    public interface IHikeRepository
    {
        Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff);
        Task<Hike?> CreateHike(Hike hike);
        Task<List<HikeResponse>> GetAllHikes();
        Task<HikeResponse> GetHike(Guid hikeID);
        Task<bool> DeleteHike(Guid hikeID);
        Task<bool> ModifyHike(Hike modifiedHike);
    }
}
