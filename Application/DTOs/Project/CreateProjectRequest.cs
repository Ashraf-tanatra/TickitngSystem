namespace ApplicationServices.DTOs.Project
{
    public class CreateProjectRequest
    {
        public DateOnly? EndTime { get; set; }
        public DateOnly? StartTime { get; set; }
        public int ProjectManagerId { get; set; }
        public string? ProjectDescription { get; set; }
        public string ProjectName { get; set; } = null!;


    }
}