using IgnitisHomework.Controllers;
using IgnitisHomework.Data;
using IgnitisHomework.DTOs;
using IgnitisHomework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
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

		private void MockModelState(PowerPlantDto dto, string key, string message)
		{
			_controller.ModelState.AddModelError(key, message);
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

		[Fact]
		public void AddPowerPlant_ValidData_Returns201Created()
		{
			var validDto = GetValidPowerPlantDto();

			var result = _controller.AddPowerPlant(validDto);

			var createdResult = Assert.IsType<CreatedAtActionResult>(result);

			var returnedDto = Assert.IsType<PowerPlantDto>(createdResult.Value);

			var savedPlant = _context.PowerPlants.FirstOrDefault(plant => plant.Id == returnedDto.Id);

			Assert.NotNull(savedPlant);
			Assert.Equal(validDto.Owner, savedPlant.Owner);
			Assert.Equal(validDto.Power!.Value, savedPlant.Power);
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

			MockModelState(incompleteDto, "Owner", "The Owner field is required.");
			MockModelState(incompleteDto, "Power", "The Power field is required.");
			MockModelState(incompleteDto, "ValidFrom", "The ValidFrom field is required.");

			var result = _controller.AddPowerPlant(incompleteDto);

			var badRequestResult = Assert.IsType<ObjectResult>(result);
			var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

			Assert.True(validationProblem.Errors.ContainsKey("Owner"));
			Assert.True(validationProblem.Errors.ContainsKey("Power"));
			Assert.True(validationProblem.Errors.ContainsKey("ValidFrom"));
		}

		[Theory]
        [InlineData("John")]
        [InlineData("John-Doe")]
        [InlineData("John Doe123")]
        public void AddPowerplant_OwnerFormatInvalid_Returns400ValidationProblem(string invalidOwner)
		{
			var dto = GetValidPowerPlantDto();
			dto.Owner = invalidOwner;

			MockModelState(dto, "Owner", "Owner must consist of exactly two words containing only letters.");

			var result = _controller.AddPowerPlant(dto);

			var badRequestResult = Assert.IsType<ObjectResult>(result);
			var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
			Assert.True(validationProblem.Errors.ContainsKey("Owner"));
		}

		[Theory]
		[InlineData(-0.01)]
		[InlineData(200.01)]
		public void AddPowerPlant_PowerOutOfRange_Returns400ValidationProblem(double invalidPower)
		{
			var dto = GetValidPowerPlantDto();
			dto.Power = invalidPower;

			MockModelState(dto, "Power", "Power must be between 0 and 200.");

			var result = _controller.AddPowerPlant(dto);

			var badRequestResult = Assert.IsType<ObjectResult>(result);
			var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

			Assert.True(validationProblem.Errors.ContainsKey("Power"));
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
		}
	}
}