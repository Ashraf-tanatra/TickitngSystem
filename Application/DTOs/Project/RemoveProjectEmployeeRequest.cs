namespace ApplicationServices.DTOs.Project
{
    public class RemoveProjectEmployeeRequest
    {
        public int ProjectId { get; set; }

        public int EmployeeId { get; set; }

        public int ActionByEmployeeId { get; set; }
    }
}
