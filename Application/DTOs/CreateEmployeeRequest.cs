public class CreateEmployeeRequest
{
    public string? FName { get; set; }

    public string? LName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public char Gender { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }

    public bool AcceptTerms { get; set; }
}