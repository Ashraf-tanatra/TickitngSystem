namespace ApplicationServices.DTOs.Project
{
    public class UpdateProjectRequest
    {
        public DateOnly? EndDate { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? ProjectDescription { get; set; }
        public string ProjectName { get; set; } = null!;
    }
}