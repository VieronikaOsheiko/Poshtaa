using System.Net.Http.Headers;
using System.Net.Http.Json;
using Apii.Dtos;
using Domain;
using Domain.Category;
using Domain.Parcels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Tests.Common;
using Xunit;

namespace TestProject1.HistoryT
{
    public class HistoryControllerTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly Parcel _mainParcel;
        private readonly Domain.History.History _mainHistory;
        private readonly Category _testCategory;

        public HistoryControllerTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _testCategory = CategoryData.DefaultCategory;
            _mainParcel = ParcelData.MainParcel(_mainUser.Id, _testCategory.Id);
            _mainHistory = HistoryData.MainHistory(_mainParcel.Id, _mainUser.Id);
        }

        [Fact]
        public async Task ShouldUpdateHistory()
        {
            // Arrange
            var updatedDataReceived = DateTime.UtcNow.AddDays(1); 
            var updatedHistoryDto = new HistoryDtos
            (
                Id: _mainHistory.Id.Value,
                ParcelId: _mainParcel.Id.Value,
                UserId: _mainUser.Id.Value,
                DataReceived: updatedDataReceived
            );

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync("Histories", updatedHistoryDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var dbHistory = await Context.Histories.FirstAsync(h => h.Id == _mainHistory.Id);
            dbHistory.Should().NotBeNull();
            
            dbHistory.DataReceived.Should().BeCloseTo(updatedDataReceived, new TimeSpan(1000));
        }
        [Fact]
        public async Task ShouldNotCreateHistoryWithInvalidData()
        {
            // Arrange
            var invalidHistoryDto = new HistoryDtos
            (
                Id: null,
                ParcelId: Guid.NewGuid(),  
                UserId: _mainUser.Id.Value,
                DataReceived: DateTime.UtcNow
            );

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("Histories/create", invalidHistoryDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();  
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);  
        }

        [Fact]
        public async Task DeleteHistory()
        {
            // Arrange
            var history = HistoryData.MainHistory(_mainParcel.Id, _mainUser.Id);
            await Context.Histories.AddAsync(history);
            await SaveChangesAsync();
            var historyId = history.Id;

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.DeleteAsync($"Histories/{historyId}");

            // Assert
            response.EnsureSuccessStatusCode();

            var historyFromDatabase = await Context.Histories
                .FirstOrDefaultAsync(h => h.Id == historyId);
            historyFromDatabase.Should().BeNull();
        }
        

        public async Task InitializeAsync()
        {
            await Context.Users.AddAsync(_mainUser);
            await Context.Categories.AddAsync(_testCategory);  
            await Context.Parcels.AddAsync(_mainParcel);
            await Context.Histories.AddAsync(_mainHistory);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Histories.RemoveRange(Context.Histories);
            Context.Parcels.RemoveRange(Context.Parcels);
            Context.Categories.RemoveRange(Context.Categories);
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }
}
