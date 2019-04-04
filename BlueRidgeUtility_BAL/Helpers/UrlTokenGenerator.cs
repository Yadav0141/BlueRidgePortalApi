using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlueRidgeUtility_BAL.Helpers
{
    public class UrlTokenGenerator : IUrlTokenGenerator
    {
        public string GenerateUrlToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var token = new byte[16];
                rng.GetBytes(token);
                return HttpServerUtility.UrlTokenEncode(token);
            }
        }
    }
}
