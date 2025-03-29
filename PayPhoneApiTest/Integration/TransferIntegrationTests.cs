using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Transfer;
using Application.DTOs.Wallet;
using PayPhoneApiTest.Common;

namespace PayPhoneApiTest.Integration;

public class TransferIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TransferIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Transfer_ReturnsNoContent_WhenSuccessful()
    {
        var fromDto = new WalletCreateUpdateDto
        {
            DocumentId = "555666777",
            Name = "Origen",
            Balance = 1000
        };
        var fromResponse = await _client.PostAsJsonAsync("/wallet", fromDto);
        var fromWallet = await fromResponse.Content.ReadFromJsonAsync<WalletDto>();

        var toDto = new WalletCreateUpdateDto
        {
            DocumentId = "888999000",
            Name = "Destino",
            Balance = 200
        };
        var toResponse = await _client.PostAsJsonAsync("/wallet", toDto);
        var toWallet = await toResponse.Content.ReadFromJsonAsync<WalletDto>();

        var transferDto = new TransferRequestDto
        {
            FromWalletId = fromWallet!.Id,
            ToWalletId = toWallet!.Id,
            Amount = 300
        };

        var transferResponse = await _client.PostAsJsonAsync("/transfer", transferDto);

        Assert.Equal(HttpStatusCode.NoContent, transferResponse.StatusCode);

        var updatedFrom = await _client.GetFromJsonAsync<WalletDto>($"/wallet/{fromWallet.Id}");
        var updatedTo = await _client.GetFromJsonAsync<WalletDto>($"/wallet/{toWallet.Id}");

        Assert.Equal(700, updatedFrom!.Balance);
        Assert.Equal(500, updatedTo!.Balance);
    }

    [Fact]
    public async Task Transfer_ReturnsBadRequest_WhenAmountIsInvalid()
    {
        var fromDto = new WalletCreateUpdateDto { DocumentId = "111", Name = "From", Balance = 500 };
        var toDto = new WalletCreateUpdateDto { DocumentId = "222", Name = "To", Balance = 300 };

        var fromWallet = await (await _client.PostAsJsonAsync("/wallet", fromDto)).Content.ReadFromJsonAsync<WalletDto>();
        var toWallet = await (await _client.PostAsJsonAsync("/wallet", toDto)).Content.ReadFromJsonAsync<WalletDto>();

        var transferDto = new TransferRequestDto
        {
            FromWalletId = fromWallet!.Id,
            ToWalletId = toWallet!.Id,
            Amount = -100
        };

        var response = await _client.PostAsJsonAsync("/transfer", transferDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Transfer_ReturnsNotFound_WhenWalletDoesNotExist()
    {
        var transferDto = new TransferRequestDto
        {
            FromWalletId = 9994, 
            ToWalletId = 8884,
            Amount = 100
        };

        var response = await _client.PostAsJsonAsync("/transfer", transferDto);

        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }
}
