namespace ApplicationServices.DTOs.Project
{
    public class UpdateProjectRequest
    {
        public string ProjectName { get; set; } = null!;
        public string? ProjectDescription { get; set; }
        public int ProjectManagerId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}