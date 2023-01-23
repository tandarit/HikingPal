using HikingPal.Models;

namespace HikingPal.Services
{
    public interface IPasswordHasherService
    {
        HashObject Hash(string password, int iteration);
        bool Check(string hash, string password);
    }
}
