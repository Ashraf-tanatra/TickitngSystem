namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        Account? GetByEmail(string email);

        bool EmailExists(string email);

        bool EmployeeExists(int employeeId); //?

        void Add(Account account);

        void Delete(Account account);
    }
}