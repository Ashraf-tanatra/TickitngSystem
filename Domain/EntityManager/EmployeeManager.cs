using ApplicationServices.DTOs;
using Domain.Interfaces;

namespace Domain.EntityManager
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeManager(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public EmployeeResponse Create(CreateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmployeeResponse> GetAll()
        {
            var employees = _repository.GetAll();

            return employees.Select(employee => new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender,
                Role = employee.Role.ToString(),
                IsDeleted = employee.IsDeleted
            });
        }

        public EmployeeResponse? GetById(int id)
        {
            var employee = _repository.GetById(id);

            if (employee == null)
                return null;

            return new EmployeeResponse
            {
                Id = employee.Id,
                FName = employee.FName,
                LName = employee.LName,
                Phone = employee.Phone,
                Gender = employee.Gender,
                Role = employee.Role.ToString(),
                IsDeleted = employee.IsDeleted
            };
        }

        public EmployeeResponse? Update(
            int id,
            UpdateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}