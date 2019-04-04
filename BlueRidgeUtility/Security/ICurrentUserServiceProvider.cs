using System;

namespace BlueRidgeUtility.Security
{
    public interface ICurrentUserServiceProvider
    {
        Guid? GetCurrentUserId();
    }
}