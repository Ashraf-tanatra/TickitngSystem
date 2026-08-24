namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Account? GetByEmail(string email);

        bool EmailExists(string email);

        bool EmployeeExists(int employeeId); //?

        void Add(Account account);

        void Update(Account account);

        void Delete(Account account);
        void Reactivate(Account account);
        void SoftDelete(Account account);
        Account? GetById(int id);
    }
}