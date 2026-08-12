namespace ApplicationServices.DTOs;

public class CreateEmployeeRequest
{
    public string FName { get; set; } = string.Empty;

    public string LName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
