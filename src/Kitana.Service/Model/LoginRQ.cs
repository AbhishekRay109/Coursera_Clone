using System.ComponentModel.DataAnnotations;

public class LoginRQ
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters long")]
    public string Password { get; set; }
}
