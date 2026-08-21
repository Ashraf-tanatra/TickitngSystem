using ApplicationServices.DTOs;
using ApplicationServices.DTOs.ApplicationServices.DTOs;

namespace ApplicationServices.Interfaces
{
    public interface IAuthManager
    {
        Task<AccountResponse> SignUp(
            SignUpRequest request);

        LoginResponse Login(
            LoginRequest request);
        public AccountResponse CreateAccountForExistingEmployee(int employeeId, ReactivateAccountRequest request);
        //void VerifyEmail(
        //    VerifyEmailRequest request);

        //Task ResendVerificationCode(
        //    ResendVerificationCodeRequest request);
    }
}