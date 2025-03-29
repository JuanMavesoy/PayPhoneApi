using System.Net;
using System.Net.Http.Json;
using Application.DTOs.MovementHistory;
using Application.DTOs.Wallet;
using PayPhoneApiTest.Common;


namespace PayPhoneApiTest.Integration;

public class MovementHistoryIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MovementHistoryIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllMovements_ReturnsOk()
    {
        var response = await _client.GetAsync("/movementhistory");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<MovementHistoryDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetMovementsByWalletId_ReturnsMovements()
    {
        var walletDto = new WalletCreateUpdateDto
        {
            DocumentId = "1112223334",
            Name = "Movimiento Wallett",
            Balance = 500
        };

        var walletResponse = await _client.PostAsJsonAsync("/wallet", walletDto);
        var walletCreated = await walletResponse.Content.ReadFromJsonAsync<WalletDto>();

        var movementDto = new MovementHistoryCreateDto
        {
            WalletId = walletCreated!.Id,
            Amount = 200,
            Type = "Credit"
        };

        var createResponse = await _client.PostAsJsonAsync("/movementhistory", movementDto);
        createResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/movementhistory/wallet/{walletCreated.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<MovementHistoryDto>>();
        Assert.NotNull(result);
        Assert.True(result.Any());
    }

    [Fact]
    public async Task CreateMovement_ReturnsCreated()
    {
        var walletDto = new WalletCreateUpdateDto
        {
            DocumentId = "999000111",
            Name = "Destino Movimiento",
            Balance = 800
        };

        var walletResponse = await _client.PostAsJsonAsync("/wallet", walletDto);
        var wallet = await walletResponse.Content.ReadFromJsonAsync<WalletDto>();

        var movementDto = new MovementHistoryCreateDto
        {
            WalletId = wallet!.Id,
            Amount = 100,
            Type = "Debit"
        };

        var response = await _client.PostAsJsonAsync("/movementhistory", movementDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<MovementHistoryDto>();
        Assert.NotNull(created);
        Assert.Equal(wallet.Id, created.WalletId);
        Assert.Equal(100, created.Amount);
    }
}