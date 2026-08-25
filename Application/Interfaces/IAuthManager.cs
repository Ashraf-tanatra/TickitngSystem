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

        //void VerifyEmail(
        //    VerifyEmailRequest request);

        //Task ResendVerificationCode(
        //    ResendVerificationCodeRequest request);
    }
}