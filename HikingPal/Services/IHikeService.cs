using HikingPal.Models;

namespace HikingPal.Services
{
    public interface IHikeService
    {
        Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff);
        Task<Hike?> CreateHike(Hike newHike, string userGuid);
        Task<List<HikeResponse>> GetHikes();
        Task<HikeResponse> GetHike(string hikeName);
        Task<bool> DeleteHike(string hikeGuid);
        Task<bool> ModifyHike(Hike hikeModify);
        
    }
}
