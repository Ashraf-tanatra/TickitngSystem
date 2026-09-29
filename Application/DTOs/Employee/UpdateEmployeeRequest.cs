using Domain.Enum;

namespace ApplicationServices.DTOs.Employee
{
    public class UpdateEmployeeRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string FName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string LName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(16, MinimumLength = 9)]
        public string Phone { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
    }
}
