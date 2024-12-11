using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Apii.Dtos;
using Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Tests.Common;
using Xunit;

namespace TestProject1.UsersT
{
    public class CreateUserTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly User _secondUser;

        public CreateUserTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _secondUser = UsersData.SecondUser();
        }

        [Fact]
        public async Task CreateUser()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var newUserDto = new UserDto
            (
                Id: null, 
                FirstName: "Test",
                LastName: "User",
                PhoneNumber: 1234567890,
                Login: "validUser",
                Password: "validPassword123"
            );
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("users", newUserDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var userFromDatabase = await Context.Users
                .FirstOrDefaultAsync(u => u.Login == newUserDto.Login);
            userFromDatabase.Should().NotBeNull();
            userFromDatabase?.Login.Should().Be(newUserDto.Login);
        }
        
        
        [Fact]
        public async Task ShouldNotCreateUserBecauseNameDuplicated()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var duplicateUserDto = new UserDto(
                Id: null,
                FirstName: _mainUser.FirstName,
                LastName: _mainUser.LastName,
                PhoneNumber: 1234567890,
                Login: "duplicateUser",
                Password: "validPassword123"
            );
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("users", duplicateUserDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
        
        
        [Fact]
        public async Task ShouldUpdateUser()
        {
            // Arrange
            var updatedFirstName = "User1";
            var request = new UserDto(
                Id: _mainUser.Id.Value,
                FirstName: updatedFirstName,
                LastName: _mainUser.LastName,
                PhoneNumber: 213,
                Login: _mainUser.Login,
                Password: "validPassword123");

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync($"users/{_mainUser.Id.Value}", request);

            // Assert
            response.EnsureSuccessStatusCode(); 

            var dbUser = await Context.Users.FirstAsync(x => x.Id == _mainUser.Id);
            dbUser.Should().NotBeNull();
            dbUser.FirstName.Should().Be(updatedFirstName); 
        }




        [Fact]
        public async Task DeleteUser()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var userToDelete = await Context.Users.FirstOrDefaultAsync(u => u.Login == _secondUser.Login);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.DeleteAsync($"users/{userToDelete.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var userFromDatabase = await Context.Users
                .FirstOrDefaultAsync(u => u.Id == userToDelete.Id);
            userFromDatabase.Should().BeNull();
        }
        
        public async Task InitializeAsync()
        {
            await Context.Users.AddRangeAsync(_mainUser, _secondUser);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }
}
