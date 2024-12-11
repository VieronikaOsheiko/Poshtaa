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

namespace TestProject1.ParcelT
{
    public class ParcelTests : BaseIntegrationTest, IAsyncLifetime
    {
        private readonly User _mainUser;
        private readonly Category _category;
        private readonly Parcel _mainParcel;

        public ParcelTests(IntegrationTestWebFactory factory) : base(factory)
        {
            _mainUser = UsersData.MainUser();
            _category = CategoryData.DefaultCategory;
            _mainParcel = ParcelData.MainParcel(_mainUser.Id, _category.Id);
        }

        [Fact]
        public async Task CreateParcel()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var newParcelDto = new ParcelDtos
            (
                Id: null,
                TrackNumber: 123,
                DateOfShipment: DateTime.UtcNow,
                AddresToCome: "Test address",
                Weight: "200",
                UserId: _mainUser.Id.Value,
                ReceiverId: _mainUser.Id.Value,
                CategoryId: _category.Id.Value,
                User: null 
            );
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("parcel", newParcelDto);

            // Assert
            response.EnsureSuccessStatusCode();

            var parcelFromDatabase = await Context.Parcels
                .FirstOrDefaultAsync(p => p.TrackNumber == newParcelDto.TrackNumber);
            parcelFromDatabase.Should().NotBeNull();
            parcelFromDatabase?.TrackNumber.Should().Be(newParcelDto.TrackNumber);
        }
        [Fact]
        public async Task ShouldNotCreateParcelBecauseUserNotAuthorized()
        {
            // Arrange
            var newParcelDto = new ParcelDtos
            (
                Id: Guid.NewGuid(),
                TrackNumber: 123456,
                DateOfShipment: DateTime.UtcNow,
                AddresToCome: "New address",
                Weight: "1000",
                UserId: _mainUser.Id.Value,
                ReceiverId: _mainUser.Id.Value,
                CategoryId: _category.Id.Value,
                User: null 
            );

            // Act
            var response = await Client.PostAsJsonAsync("parcel", newParcelDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();  
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);  
        }
        [Fact]
        public async Task ShouldNotCreateParcelBecauseWeightAndAddressDuplicate()
        {
            // Arrange
            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            var duplicateParcelDto = new ParcelDtos
            (
                Id: null,
                TrackNumber: 123456,  
                DateOfShipment: DateTime.UtcNow,
                AddresToCome: _mainParcel.AddresToCome, 
                Weight: _mainParcel.Weight,
                UserId: _mainUser.Id.Value,
                ReceiverId: _mainUser.Id.Value,
                CategoryId: _category.Id.Value,
                User: null 
            );
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PostAsJsonAsync("parcel", duplicateParcelDto);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();  
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict); 
        }


        [Fact]
        public async Task ShouldUpdateParcel()
        {
            // Arrange
            var updatedWeight = "250"; 
            var updatedTrackNumber = 456;
            var request = new ParcelDtos(
                
                Id: _mainParcel.Id.Value,
                TrackNumber: updatedTrackNumber,
                DateOfShipment: _mainParcel.DateOfShipment,
                AddresToCome: _mainParcel.AddresToCome,
                Weight: updatedWeight,
                UserId: _mainUser.Id.Value,
                ReceiverId: _mainUser.Id.Value,
                CategoryId: _category.Id.Value,
                User: null 
            );

            var authToken = await GenerateAuthTokenAsync(_mainUser.Login, _mainUser.Password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            // Act
            var response = await Client.PutAsJsonAsync($"parcel", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var dbParcel = await Context.Parcels.FirstAsync(x => x.Id == _mainParcel.Id);
            dbParcel.Should().NotBeNull();
            dbParcel.Weight.Should().Be(updatedWeight);
            dbParcel.TrackNumber.Should().Be(updatedTrackNumber);
        }

        public async Task InitializeAsync()
        {
            await Context.Users.AddAsync(_mainUser);
            await Context.Categories.AddAsync(_category);
            await Context.Parcels.AddAsync(_mainParcel);
            await SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            Context.Parcels.RemoveRange(Context.Parcels);
            Context.Categories.RemoveRange(Context.Categories);
            Context.Users.RemoveRange(Context.Users);
            await SaveChangesAsync();
        }
    }
}
