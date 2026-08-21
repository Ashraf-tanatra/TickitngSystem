namespace ApplicationServices.DTOs.Project
{
    public class CreateProjectRequest
    {
        public string ProjectName { get; set; } = null!;
        public string? ProjectDescription { get; set; }
        public int ProjectManagerId { get; set; }
        public DateOnly? StartTime { get; set; }
        public DateOnly? EndTime { get; set; }


    }
}