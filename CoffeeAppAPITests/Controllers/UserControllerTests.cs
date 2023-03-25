using CoffeeAppAPI.Controllers;
using CoffeeAppAPIModels = CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.AspNetCore.Mvc;

using Moq;
using System;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace CoffeeAppAPITests.Controllers
{
    public class UserControllerTests
    {
        private readonly UsersController _controller;
        private readonly Mock<ICosmosDbService> _cosmosDbServiceMock;
        private readonly Container _mockContainer;

        public UserControllerTests()
        {
            _cosmosDbServiceMock = new Mock<ICosmosDbService>();
            _mockContainer = new Mock<Container>().Object; // Create a mocked Container object

            // Set up the mock method after initializing the mock object
            _cosmosDbServiceMock.Setup(s => s.GetOrCreateContainerAsync("Users", "/id")).ReturnsAsync(_mockContainer);

            _controller = new UsersController(_cosmosDbServiceMock.Object);
        }
        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _cosmosDbServiceMock.Setup(s => s.GetItemAsync<CoffeeAppAPIModels.User>(_mockContainer, It.IsAny<string>())).ReturnsAsync((CoffeeAppAPIModels.User)null);


            // Act
            var result = await _controller.GetUser(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            var user = new CoffeeAppAPIModels.User
            {
                id = userId,
                Username = "johndoe",
                Email = "john.doe@example.com",
                Password = "securePassword123",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Coffee enthusiast and software developer.",
                ProfilePictureUrl = "https://example.com/profile_pictures/johndoe.jpg",
                JoinDate = DateTime.UtcNow,
                TotalCheckins = 10,
                TotalUniqueCoffees = 5,
                TotalBadges = 3,
                FavoriteCoffeeShops = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Friends = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _cosmosDbServiceMock.Setup(s => s.GetItemAsync<CoffeeAppAPIModels.User>(_mockContainer, userId.ToString())).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(userId.ToString());

            // Debug: Print the result
            Console.WriteLine($"Result: {result.Result}");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<CoffeeAppAPIModels.User>(okResult.Value);
            Assert.Equal(user, returnedUser);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateUser(new CoffeeAppAPIModels.User());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            var user = new CoffeeAppAPIModels.User
            {
                id = Guid.NewGuid(),
                Username = "johndoe",
                Email = "john.doe@example.com",
                Password = "securePassword123",
                FirstName = "John",
                LastName = "Doe",
                Bio = "Coffee enthusiast and software developer.",
                ProfilePictureUrl = "https://example.com/profile_pictures/johndoe.jpg",
                JoinDate = DateTime.UtcNow,
                TotalCheckins = 10,
                TotalUniqueCoffees = 5,
                TotalBadges = 3,
                FavoriteCoffeeShops = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Friends = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            _cosmosDbServiceMock.Setup(s => s.AddItemAsync(_mockContainer, user)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetUser), ((CreatedAtActionResult)result.Result).ActionName);
        }
    }
}
