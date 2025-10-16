using TvShowTracker.Api.Services;
using TvShowTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TvShowTracker.Api.Models;
using Xunit;

namespace TvShowTracker.Tests.tests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _context;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("AuthTestDb")
                .Options;

            _context = new AppDbContext(options);
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

            _authService = new AuthService(config, _context);
        }

        [Fact]
        public async Task RegisterUser_CreatesUserInDatabase()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";

            // Act
            var user = await _authService.RegisterAsync(username, password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Single(_context.Users);
        }

        [Fact]
        public async Task LoginUser_ReturnsJwtToken()
        {
            // Arrange
            var username = "loginuser";
            var password = "mypassword";
            await _authService.RegisterAsync(username, password);

            // Act
            var token = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public async Task LoginUser_InvalidPassword_ThrowsException()
        {
            // Arrange
            var username = "wrongpass";
            await _authService.RegisterAsync(username, "correct");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(username, "incorrect"));
        }
    }
}
