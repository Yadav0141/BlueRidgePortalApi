using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
    public class HashedPassword
    {
        public HashedPassword(
            byte? version,
            string salt,
            string hash)
        {
            Version = version;
            Salt = salt;
            Hash = hash;
        }

        public byte? Version { get; }
        public string Salt { get; }
        public string Hash { get; }
    }
}
