using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using IgnitisHomework.Models;
using IgnitisHomework.Data;
using IgnitisHomework.DTOs;
using IgnitisHomework.Helpers;

using System.ComponentModel.DataAnnotations;

namespace IgnitisHomework.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PowerPlantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PowerPlantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? owner, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            IQueryable<Models.PowerPlant> plantsQuery = _context.PowerPlants;

            if (!string.IsNullOrWhiteSpace(owner))
            {
                var cleanedSearchString = StringHelpers.RemoveDiacritics(owner).ToLower().Trim();

                plantsQuery = plantsQuery.Where(plant =>
                    AppDbContext.Unaccent(plant.Owner).ToLower().Contains(cleanedSearchString)
                );
            }

            plantsQuery = plantsQuery.OrderBy(plant => plant.Id);
            var totalCount = plantsQuery.Count();

            pageNumber = Math.Max(1, pageNumber);

            var skipAmount = (pageNumber - 1) * pageSize;

            plantsQuery = plantsQuery.Skip(skipAmount).Take(pageSize);

            var plants = plantsQuery
                .Select(plant => DTOs.PowerPlantDto.FromEntity(plant))
                .ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var wrapper = new PowerPlantsWrapperDto { 
                PowerPlants = plants,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return Ok(wrapper);
        }

        [HttpPost]
        public IActionResult AddPowerPlant([FromBody] PowerPlantDto plantDto)
        {
            if (plantDto == null)
                return BadRequest("Power plant data is required");

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
