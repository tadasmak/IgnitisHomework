using System.ComponentModel.DataAnnotations;

public class TwoWordLettersOnlyAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		if (value is not string ownerString)
		{
			return ValidationResult.Success;
		}

		var parts = ownerString.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2 || parts.Any(part => !part.All(char.IsLetter)))
		{
			return new ValidationResult(
				ErrorMessage ?? $"{validationContext.DisplayName} must be two words, letters only.",
				new[] { validationContext.MemberName! }
			);
		}

		return ValidationResult.Success;
	}
}