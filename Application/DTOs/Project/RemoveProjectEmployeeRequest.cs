namespace ApplicationServices.DTOs.Project
{
    public class RemoveProjectEmployeeRequest
    {
        public Guid ProjectId { get; set; }

        public Guid EmployeeId { get; set; }

    }
}
