using ApplicationServices.DTOs;

namespace ApplicationServices.Interfaces
{
    public interface IAuthManager
    {
        AccountResponse SignUp(SignUpRequest request);

        LoginResponse Login(LoginRequest request);
    }
}