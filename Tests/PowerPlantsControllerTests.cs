using IgnitisHomework.Controllers;
using IgnitisHomework.Data;
using IgnitisHomework.DTOs;
using IgnitisHomework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace IgnitisHomework.Tests
{
	public class PowerPlantsControllerTests : IDisposable
	{
		private readonly AppDbContext _context;
		private readonly PowerPlantsController _controller;

		public PowerPlantsControllerTests()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new AppDbContext(options);
			_controller = new PowerPlantsController(_context);
		}

		private PowerPlantDto GetValidPowerPlantDto()
		{
			return new PowerPlantDto
			{
				Id = 0,
				Owner = "John Doe",
				Power = 50.0,
				ValidFrom = DateTime.UtcNow.AddDays(-1),
				ValidTo = null
			};
		}

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
            return validationResults;
        }

        private void SeedDatabase()
        {
            _context.PowerPlants.AddRange(new[]
            {
                new PowerPlant { Id = 1, Owner = "Vardenis Pavardenis", Power = 9.3, ValidFrom = new DateTime(2020, 1, 1), ValidTo = new DateTime(2025, 1, 1) },
                new PowerPlant { Id = 2, Owner = "Jonas Jonaitis", Power = 5.7, ValidFrom = new DateTime(2021, 6, 15), ValidTo = new DateTime(2026, 6, 15) },
                new PowerPlant { Id = 3, Owner = "Ona Petraitë", Power = 12.5, ValidFrom = new DateTime(2019, 9, 10), ValidTo = null }
            });
            _context.SaveChanges();
        }

        [Fact]
		public void AddPowerPlant_ValidData_Returns201Created()
		{
			var validDto = GetValidPowerPlantDto();

			var result = _controller.AddPowerPlant(validDto);

			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			Assert.Equal(201, createdResult.StatusCode);

			var returnedDto = Assert.IsType<PowerPlantDto>(createdResult.Value);
			Assert.True(returnedDto.Id > 0);

			var savedPlant = _context.PowerPlants.FirstOrDefault(plant => plant.Id == returnedDto.Id);

			Assert.NotNull(savedPlant);
			Assert.Equal(validDto.Owner, savedPlant.Owner);
			Assert.Equal(validDto.Power!.Value, savedPlant.Power);
			Assert.Equal(validDto.ValidFrom!.Value, savedPlant.ValidFrom);
		}

		[Fact]
		public void AddPowerPlant_NullBody_Returns400BadRequest()
		{
			PowerPlantDto? nullDto = null;

			var result = _controller.AddPowerPlant(nullDto!);

			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Power plant data is required", badRequestResult.Value);
		}

		[Fact]
		public void AddPowerPlant_MissingRequiredFields_Returns400ValidationProblem()
		{
			var incompleteDto = new PowerPlantDto
			{
				Owner = null!,
				Power = null,
				ValidFrom = default
			};

			var validationResults = ValidateModel(incompleteDto);

            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(PowerPlantDto.Owner)));
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(PowerPlantDto.Power)));
            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(PowerPlantDto.ValidFrom)));
        }

        [Theory]
        [InlineData("John")]
        [InlineData("John-Doe")]
        [InlineData("John Doe123")]
        [InlineData("John Paul Doe")]
        public void PowerPlantDto_OwnerFormatInvalid_FailsValidation(string invalidOwner)
		{
			var dto = GetValidPowerPlantDto();
			dto.Owner = invalidOwner;

			var validationResults = ValidateModel(dto);

			Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(PowerPlantDto.Owner)));
		}

		[Theory]
		[InlineData(-0.01)]
		[InlineData(200.01)]
		public void PowerPlantDto_PowerOutOfRange_FailsValidation(double invalidPower)
		{
			var dto = GetValidPowerPlantDto();
			dto.Power = invalidPower;

            var validationResults = ValidateModel(dto);

            Assert.Contains(validationResults, r => r.MemberNames.Contains(nameof(PowerPlantDto.Power)));
        }

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
		}
	}
}