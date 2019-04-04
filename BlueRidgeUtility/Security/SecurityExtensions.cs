using Microsoft.IdentityModel.Tokens;
using System;

using System.Linq;
using System.Text;
using System.Web;

namespace BlueRidgeUtility.Security
{
    public static class SecurityExtensions
    {
        public static SigningCredentials ToIdentitySigningCredentials(this string jwtSecret)
        {
            var symmetricKey = jwtSecret.ToSymmetricSecurityKey();
            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);

            return signingCredentials;
        }

        public static SymmetricSecurityKey ToSymmetricSecurityKey(this string jwtSecret)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        }
    }
}