using System.Net.Http.Headers;
using System.Net.Http.Json;
using Apii.Dtos;
using Domain;
using Domain.Category;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Tests.Common;
using Xunit;

namespace TestProject1.CategoryT
{
    public class CategoryTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly Category _testCategory;

        public CategoryTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _testCategory = CategoryData.DefaultCategory;
        }

        [Fact]
        public async Task CreateCategory()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var newCategoryDto = new CategoryDtos
            (
                Id: null,
                Name: "New Category",
                Material: "Metal",
                InCountry: true,
                Size: "Large"
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("category", newCategoryDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var categoryFromDatabase = await Context.Categories
                .FirstOrDefaultAsync(c => c.Name == newCategoryDto.Name);

            categoryFromDatabase.Should().NotBeNull();
            categoryFromDatabase?.Name.Should().Be(newCategoryDto.Name);
        }
        [Fact]
        public async Task ShouldNotCreateCategoryBecauseUserNotAuthorized()
        {
            // Arrange
            var newCategoryDto = new CategoryDtos
            (
                Id: null,
                Name: "Unauthorized Category",
                Material: "Glass",
                InCountry: true,
                Size: "Medium"
            );
            // Act
            var response = await Client.PostAsJsonAsync("category", newCategoryDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();  
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);  
        }
        [Fact]
        public async Task ShouldNotCreateCategoryBecauseNameAlreadyExists()
        {
            // Arrange
            await Context.Categories.AddAsync(_testCategory);  
            await SaveChangesAsync();

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var newCategoryDto = new CategoryDtos
            (
                Id: null,
                Name: _testCategory.Name,  
                Material: "Plastic",
                InCountry: true,
                Size: "Small"
            );

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("category", newCategoryDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();  
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);  
        }

        [Fact]
        public async Task DeleteCategory()
        {
            // Arrange
            await Context.Categories.AddAsync(_testCategory);
            await SaveChangesAsync();

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.DeleteAsync($"category/{_testCategory.Id.Value}");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var deletedCategory = await Context.Categories
                .FirstOrDefaultAsync(c => c.Id == _testCategory.Id);

            deletedCategory.Should().BeNull();
        }

        public async Task InitializeAsync()
        {
            await Context.Users.AddAsync(_mainUser);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Users.RemoveRange(Context.Users);
            Context.Categories.RemoveRange(Context.Categories);
            await SaveChangesAsync();
        }
    }
}
