using Boiler.Auth.Entities;
using Boiler.Auth.Helpers;
using Boiler.Auth.Interfaces;
using Boiler.Auth.RequestModels;
using Boiler.Auth.Services;
using Boiler.Util.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Linq;
using Xunit;

namespace Boiler.Auth.Tests
{
    public class AuthControllerTests
    {
        public ServiceProvider ServiceProvider { get; set; }
        public IConfiguration Configuration { get; set; }

        public AuthControllerTests()
        {
            // Set Config
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<AuthServiceTests>()
                .AddEnvironmentVariables();
            Configuration = configBuilder.Build();

            var mockLogger = new Mock<ILogger>();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<MockContext>(options =>
                options.UseInMemoryDatabase(databaseName: "TestDb"));
            serviceCollection.Configure<AuthSettings>(Configuration.GetSection("Auth"));
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<IAuthContext>(provider => provider.GetService<MockContext>());
            serviceCollection.AddScoped(provider => mockLogger.Object);
            serviceCollection.AddScoped(provider => Configuration);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            SeedDatabase();
        }

        [Fact]
        public void Login_ReturnsToken()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test0@test.com",
                Password = "1234"
            };
            using var scope = ServiceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            var response = authService.Login(loginRequest, "127.0.0.1");
            Assert.True(!string.IsNullOrEmpty(response.JwtToken));
        }

        [Fact]
        public void Login_TokenIdClaimIsValid()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test0@test.com",
                Password = "1234"
            };
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<MockContext>();
            var account = context.Accounts.FirstOrDefault(x => x.Email == loginRequest.Email);
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            var response = authService.Login(loginRequest, "127.0.0.1");
            var id = authService.GetIdFromToken(response.JwtToken);
            Assert.True(id == account.Id);
        }

        [Fact]
        public void Login_ThrowsExceptionOnInvalidEmail()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test0@wrong.com",
                Password = "1234"
            };
            using var scope = ServiceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            Assert.Throws<AppException>(() =>
            {
                var response = authService.Login(loginRequest, "127.0.0.1");
            });
        }

        [Fact]
        public void Login_ThrowsExceptionOnInvalidPassword()
        {
            var loginRequest = new LoginRequest
            {
                Email = "test0@wrong.com",
                Password = "12345"
            };
            using var scope = ServiceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            Assert.Throws<AppException>(() =>
            {
                var response = authService.Login(loginRequest, "127.0.0.1");
            });
        }

        [Fact]
        public void Register_ReturnsToken()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "new_user@test.com",
                Password = "1234",
                PasswordConfirmation = "1234",
            };
            using var scope = ServiceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            var response = authService.Register(registerRequest);
            Assert.True(!string.IsNullOrEmpty(response.JwtToken));
        }

        [Fact]
        public void Register_ThrowsErrorOnExistingUser()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "test0@test.com",
                Password = "1234",
                PasswordConfirmation = "1234",
            };
            using var scope = ServiceProvider.CreateScope();
            var authService = scope.ServiceProvider.GetService<IAuthService>();
            var exception = Assert.Throws<AppException>(() =>
            {
                var response = authService.Register(registerRequest);
            });
            Assert.Equal("User is already registered!", exception.Message);
        }

        void SeedDatabase()
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<MockContext>();
            context.Accounts.Add(AuthService.CreateTestAccount("admin@test.com", "1234", Role.Admin));
            context.Accounts.Add(AuthService.CreateTestAccount("test0@test.com", "1234", Role.User));
            context.Accounts.Add(AuthService.CreateTestAccount("test1@test.com", "1234", Role.User));
            context.SaveChanges();
        }
    }
}
