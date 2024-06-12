using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Service.Model
{

    public class UserRQ
    {
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [Password(ErrorMessage = "Password must meet the complexity requirements")]
        public string PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
    enum RoleTypes
    {
        Admin,
        Instructor,
        EndUser
    };
    public class PasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;
            if (password == null)
            {
                return true;
            }
            // Check if the password meets the required criteria
            return password.Length >= 5 &&
                   password.Length <= 16 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(name);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (password == null)
            {
                return ValidationResult.Success;
            }

            bool flag = false;
            string errorMessage = "Password must contain at least";

            if (password.Length < 5 || password.Length > 16)
            {
                throw new ArgumentException("Password must be between 5 and 16 characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                errorMessage = errorMessage + (" one uppercase letter,");
                flag = true;
            }

            if (!password.Any(char.IsLower))
            {
                errorMessage = errorMessage + (" one lowercase letter,");
                flag = true;
            }

            if (!password.Any(char.IsDigit))
            {
                errorMessage = errorMessage + (" one digit,");
                flag = true;
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errorMessage = errorMessage + (" one special character,");
                flag = true;
            }

            if (flag)
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 1, 1) + ".";
                throw new ArgumentException(errorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
