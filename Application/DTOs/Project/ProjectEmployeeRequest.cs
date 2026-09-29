namespace ApplicationServices.DTOs.Project
{
    public class ProjectEmployeeRequest
    {
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string? Role { get; set; }
    }
}
