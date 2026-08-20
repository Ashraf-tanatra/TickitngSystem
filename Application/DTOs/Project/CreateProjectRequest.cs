namespace ApplicationServices.DTOs.Project
{
    public class CreateProjectRequest
    {
        public string ProjectName { get; set; } = null!;
        public string? ProjectDescription { get; set; }
        public int ProjectManagerId { get; set; } // The employee who created the project
    }
}