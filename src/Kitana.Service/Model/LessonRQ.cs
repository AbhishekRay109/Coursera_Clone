using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class LessonRQ
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "A video file is required")]
    [MaxFileSize(1024 * 1024 * 500, ErrorMessage = "Maximum allowed file size is 500 MB")]
    [AllowedExtensions(new string[] { ".mp4", ".avi", ".mov" }, ErrorMessage = "Only .mp4, .avi, and .mov formats are allowed")]
    public IFormFile VideoFile { get; set; }
}

// Custom Validation Attribute for Maximum File Size
public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly long _maxFileSize;

    public MaxFileSizeAttribute(long maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            if (file.Length > _maxFileSize)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedExtensionsAttribute(string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

            if (string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
