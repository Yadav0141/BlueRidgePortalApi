using BlueRidgeUtility_BAL.Models;

namespace BlueRidgeUtility_BAL.Helpers
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, string salt);
        HashedPassword HashPassword(string password);
    }
}