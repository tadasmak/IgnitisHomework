using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using IgnitisHomework.Models;
using IgnitisHomework.Data;
using IgnitisHomework.DTOs;

using System.ComponentModel.DataAnnotations;

namespace IgnitisHomework.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PowerPlantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PowerPlantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var plants = _context.PowerPlants
                                 .Select(plant => DTOs.PowerPlantDto.FromEntity(plant))
                                 .ToList();

            var wrapper = new PowerPlantsWrapperDto { PowerPlants = plants };

            return Ok(wrapper);
        }

        [HttpPost]
        public IActionResult AddPowerPlant([FromBody] PowerPlantDto plantDto)
        {
            if (plantDto == null)
                return BadRequest("PowerPlant data is required");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var newPlant = new Models.PowerPlant
                {
                    Owner = plantDto.Owner,
                    Power = plantDto.Power ?? 0,
                    ValidFrom = DateTime.SpecifyKind(plantDto.ValidFrom, DateTimeKind.Utc),
                    ValidTo = plantDto.ValidTo.HasValue ? DateTime.SpecifyKind(plantDto.ValidFrom, DateTimeKind.Utc) : null
                };

                _context.PowerPlants.Add(newPlant);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetAll), new { id = newPlant.Id }, PowerPlantDto.FromEntity(newPlant));
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected server error occurred during data saving.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var plant = _context.PowerPlants.Find(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.PowerPlants.Remove(plant);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
