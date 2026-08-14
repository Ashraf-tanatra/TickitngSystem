using ApplicationServices.DTOs;
using ApplicationServices.Interfaces;
using Domain.Interfaces;

namespace ApplicationServices.Managers
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeManager(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public EmployeeResponse Create(CreateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }

        public EmployeeResponse? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmployeeResponse> GetAll()
        {
            throw new NotImplementedException();
        }

        public EmployeeResponse? Update(
            int id,
            UpdateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}