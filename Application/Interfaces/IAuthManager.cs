using ApplicationServices.DTOs.Account;

namespace ApplicationServices.Interfaces
{
    public interface IAuthManager
    {
        Task VerifyEmail(VerifyEmailRequest request);
        Task<LoginResponse> Login(LoginRequest request);
        Task ResetPassword(ResetPasswordRequest request);
        Task ForgotPassword(ForgotPasswordRequest request);
        Task<AccountResponse> SignUp(SignUpRequest request);
    }
}