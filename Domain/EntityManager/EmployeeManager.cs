using ApplicationServices.DTOs;
using Domain.Interfaces;

namespace Domain.EntityManager
{
    public class EmployeeManager : IEmployeeManager
    {
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
            throw new NotImplementedException();
        }

        public EmployeeResponse? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public EmployeeResponse? Update(int id, UpdateEmployeeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}