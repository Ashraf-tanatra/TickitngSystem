namespace ApplicationServices.DTOs.Project
{
    public class ProjectEmployeeRequest
    {
        public Guid ProjectId { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Role { get; set; }
    }
}
