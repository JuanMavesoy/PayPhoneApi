using Application.DTOs.Wallet;
using PayPhoneApiTest.Common;
using System.Net.Http.Json;
using System.Net;


namespace PayPhoneApiTest.Integration
{
    public class WalletIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WalletIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllWallets_ReturnsList()
        {
            var response = await _client.GetAsync("/wallet");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var wallets = await response.Content.ReadFromJsonAsync<List<WalletDto>>();
            Assert.NotNull(wallets);
            Assert.True(wallets.Count >= 0);
        }

        [Fact]
        public async Task GetWalletById_ReturnsWallet()
        {
            var dto = new WalletCreateUpdateDto
            {
                DocumentId = "123123123",
                Name = "Consulta Individual",
                Balance = 300
            };

            var createResponse = await _client.PostAsJsonAsync("/wallet", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

            var response = await _client.GetAsync($"/wallet/{created!.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var wallet = await response.Content.ReadFromJsonAsync<WalletDto>();
            Assert.NotNull(wallet);
            Assert.Equal(created.Id, wallet!.Id);
        }


        [Fact]
        public async Task CreateWallet_ReturnsCreatedAndWalletData()
        {
            var dto = new WalletCreateUpdateDto
            {
                DocumentId = "1122334455",
                Name = "Integración Test",
                Balance = 750m
            };

            var response = await _client.PostAsJsonAsync("/wallet", dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var wallet = await response.Content.ReadFromJsonAsync<WalletDto>();
            Assert.NotNull(wallet);
            Assert.Equal("Integración Test", wallet!.Name);
            Assert.Equal("1122334455", wallet.DocumentId);
            Assert.Equal(750m, wallet.Balance);
        }


        [Fact]
        public async Task UpdateWallet_ReturnsUpdatedWallet()
        {
            var dto = new WalletCreateUpdateDto
            {
                DocumentId = "456456456",
                Name = "Original",
                Balance = 200
            };

            var createResponse = await _client.PostAsJsonAsync("/wallet", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

            var updateDto = new WalletCreateUpdateDto
            {
                DocumentId = "456456456",
                Name = "Actualizado",
                Balance = 1000
            };

            var updateResponse = await _client.PutAsJsonAsync($"/wallet/{created!.Id}", updateDto);

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updated = await updateResponse.Content.ReadFromJsonAsync<WalletDto>();
            Assert.NotNull(updated);
            Assert.Equal("Actualizado", updated!.Name);
            Assert.Equal(1000, updated.Balance);
        }

        [Fact]
        public async Task DeleteWallet_RemovesWallet()
        {
            var dto = new WalletCreateUpdateDto
            {
                DocumentId = "000111222",
                Name = "Temporal",
                Balance = 100
            };

            var createResponse = await _client.PostAsJsonAsync("/wallet", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<WalletDto>();

            var deleteResponse = await _client.DeleteAsync($"/wallet/{created!.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/wallet/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }



    }
}
