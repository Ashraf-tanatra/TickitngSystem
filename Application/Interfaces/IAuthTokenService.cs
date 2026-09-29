using ApplicationServices.DTOs.Account;
using Domain.Entities;

namespace ApplicationServices.Interfaces;

public interface IAuthTokenService
{
    AuthToken Create(Account account);
}
