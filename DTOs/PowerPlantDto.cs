using System.ComponentModel.DataAnnotations;

namespace IgnitisHomework.DTOs
{
    public class PowerPlantDto
    {
        public int Id { get; set; }
        [Required] public string Owner { get; set; } = string.Empty;
        [Required] public double? Power { get; set; }    
        [Required] public string ValidFrom { get; set; } = string.Empty;
        public string? ValidTo { get; set; }

        public static PowerPlantDto FromEntity(Models.PowerPlant plant)
        {
            return new PowerPlantDto
            {
                Id = plant.Id,
                Owner = plant.Owner,
                Power = plant.Power,
                ValidFrom = plant.ValidFrom.ToString("yyyy-MM-dd"),
                ValidTo = plant.ValidTo?.ToString("yyyy-MM-dd")
            };
        }
    }
}