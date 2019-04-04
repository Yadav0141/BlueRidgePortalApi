using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace BlueRidgeUtility.Security
{
    public class CurrentUserServiceProvider : ICurrentUserServiceProvider
    {
        public Guid? GetCurrentUserId() {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            if (identity != null)
            {
                var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                  .Select(c => c.Value).SingleOrDefault();
                return new Guid(userId);
            }
           return null;
        }
    }
}