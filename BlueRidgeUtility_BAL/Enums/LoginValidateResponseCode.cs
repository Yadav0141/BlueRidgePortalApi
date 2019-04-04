using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Enums
{
    public enum LoginValidateResponseCode
    {
        INVALID_USERNAME_PASSWORD,
        USER_NOT_EXIST,
        USER_LOGIN_SUCCESS,
        PASSWORD_NOT_SET
    }

    public enum ForgotPasswordResponseCode {
        USER_NOT_FOUND,
        USER_EXIST
    }
}
