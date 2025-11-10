using System.ComponentModel.DataAnnotations;

namespace IgnitisHomework.Models
{
    public class PowerPlant
    {
        [Key] public int Id { get; set; }

        [Required]
        [TwoWordLettersOnly]
        public string Owner { get; set; } = string.Empty;

        [Required]
        [Range(0.0, 200.0, ErrorMessage = "Power must be between 0 and 200.")]
        public double Power { get; set; }

        [Required] public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}