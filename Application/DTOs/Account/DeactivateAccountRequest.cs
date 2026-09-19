namespace ApplicationServices.DTOs.Account
{
    public class DeactivateAccountRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(128)]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}
