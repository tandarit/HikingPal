using HikingPal.Models;

namespace HikingPal.Services
{
    public interface IHikeService
    {
        Task<bool> SubscribeHike(HikeUser hikeUser, bool onOff);
        Task<HikeDTO?> CreateHike(HikeDTO newHike, string userGuid);
        Task<List<HikeDTO>> GetHikes();
        Task<HikeDTO> GetHike(string hikeName);
        Task<bool> DeleteHike(string hikeGuid);
        Task<bool> ModifyHike(Hike hikeModify);
        
    }
}
