namespace ApplicationServices.DTOs.Project
{
    public class ProjectEmployeeRequest
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string? Role { get; set; }
    }
}
