namespace Assignment2.APP.Models;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Fail(string error) => new()
    {
        IsValid = false,
        Errors = new List<string> { error }
    };

    public static ValidationResult Fail(List<string> errors) => new()
    {
        IsValid = false,
        Errors = errors
    };
}
