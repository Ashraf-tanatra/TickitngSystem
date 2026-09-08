using ApplicationServices.DTOs.Account;
using ApplicationServices.DTOs.ApplicationServices.DTOs;

namespace ApplicationServices.Interfaces
{
    public interface IAuthManager
    {
        Task<AccountResponse> SignUp(
            SignUpRequest request);

        Task<LoginResponse> Login(
            LoginRequest request);

        Task VerifyEmail(VerifyEmailRequest request);

        Task ResendVerificationCode(ForgotPasswordRequest request);

        Task ForgotPassword(ForgotPasswordRequest request);

        Task VerifyResetCode(VerifyResetCodeRequest request);

        Task ResetPassword(ResetPasswordRequest request);
    }
}
