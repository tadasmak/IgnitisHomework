using System.ComponentModel.DataAnnotations;

namespace IgnitisHomework.Models
{
    public class PowerPlant
    {
        [Key] public int Id { get; set; }

        [Required] public string Owner { get; set; } = string.Empty;
        
        [Required] public double Power { get; set; }

        [Required] public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Power < 0 || Power > 200)
                yield return new ValidationResult("Power must be between 0 and 200", new[] { nameof(Power) });

            if (string.IsNullOrWhiteSpace(Owner))
                yield return new ValidationResult("Owner is required", new[] { nameof(Owner) });
            else
            {
                var parts = Owner.Trim().Split(' ');
                if (parts.Length != 2 || parts.Any(parts => !parts.All(char.IsLetter)))
                    yield return new ValidationResult("Owner must be two words, letters only", new[] { nameof(Owner) });
            }
        }
    }
}