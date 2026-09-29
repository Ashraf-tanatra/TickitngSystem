namespace ApplicationServices.DTOs.Project
{
    public class UpdateProjectRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(125)]
        public string ProjectName { get; set; } = null!;

        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string? ProjectDescription { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
