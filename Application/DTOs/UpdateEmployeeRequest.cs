using Domain.Enum;

namespace ApplicationServices.DTOs
{
    public class UpdateEmployeeRequest
    {
        public string? FName { get; set; }

        public string? LName { get; set; }

        public string? Phone { get; set; }

        public Gender Gender { get; set; }
    }
}