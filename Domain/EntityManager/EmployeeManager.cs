using ApplicationServices.DTOs;
using Domain.Interfaces;



namespace Domain.EntityManager
{
    public class EmployeeManager : IEmployeeManager
    {
        public Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmployeeResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeResponse?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}