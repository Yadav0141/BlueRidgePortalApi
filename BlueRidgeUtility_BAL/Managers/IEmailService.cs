using BlueRidgeUtility_BAL.Models;

namespace BlueRidgeUtility_BAL.Managers
{
    public interface IEmailService
    {
        void sendResetPasswordEmail(ForgotPasswordEmailModel model);
    }
}