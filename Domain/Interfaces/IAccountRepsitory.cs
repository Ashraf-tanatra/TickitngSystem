using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();

        Account? GetById(int id);

        Account? GetByEmail(string email);

        void Add(Account account);

        void Update(Account account);

        void Delete(Account account);
    }
}