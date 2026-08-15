namespace ApplicationServices.DTOs
{
    public class CreateProjectRequest
    {
        public string ProjectName { get; set; } = string.Empty;

        public string? ProjectDescription { get; set; }

        public int ProjectManagerId { get; set; }
    }
}