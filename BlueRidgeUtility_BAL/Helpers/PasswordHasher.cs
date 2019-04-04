using BlueRidgeUtility_BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlueRidgeUtility_BAL.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltLength = 16;
        private const int HashLength = 32;
        private const byte CurrentVersion = 1;

        private readonly IDictionary<byte, int> _versionIterations =
            new Dictionary<byte, int>
                {
                    {1, 8000}
                };

        public HashedPassword HashPassword(string password)
        {
            using (var hasher = new Rfc2898DeriveBytes(password, SaltLength, _versionIterations[CurrentVersion]))
            {
                return new HashedPassword(
                    CurrentVersion,
                    Convert.ToBase64String(hasher.Salt),
                    Convert.ToBase64String(hasher.GetBytes(HashLength)));
            }
        }

        public string HashPassword(string password, string salt)
        {
            return ComputeVersion1Hash(password, salt);
          
        }

        private string ComputeVersion1Hash(string password, string salt)
        {
            using (var hasher = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), _versionIterations[1]))
            {
                return Convert.ToBase64String(hasher.GetBytes(HashLength));
            }
        }
    }
}