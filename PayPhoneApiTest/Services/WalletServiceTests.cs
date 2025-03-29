using Application.DTOs.Wallet;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace PayPhoneApiTest.Services
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepoMock;
        private readonly WalletService _service;

        public WalletServiceTests()
        {
            _walletRepoMock = new Mock<IWalletRepository>();
            _service = new WalletService(_walletRepoMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsWalletDto_WhenExists()
        {
            var wallet = CreateSampleWallet();
            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(wallet);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(wallet.Id, result!.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            _walletRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Wallet?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfWallets()
        {
            var wallets = new List<Wallet> { CreateSampleWallet() };
            _walletRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(wallets);

            var result = await _service.GetAllAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsync_AddsAndReturnsWallet()
        {
            var dto = new WalletCreateUpdateDto
            {
                DocumentId = "ABC123",
                Name = "Test User",
                Balance = 500m
            };

            Wallet? captured = null;

            _walletRepoMock.Setup(r => r.AddAsync(It.IsAny<Wallet>()))
                .Callback<Wallet>(w => captured = w)
                .Returns(Task.CompletedTask);

            _walletRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.DocumentId, result.DocumentId);
            Assert.NotNull(captured);
            Assert.Equal(dto.Balance, captured!.Balance);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesWallet_WhenExists()
        {
            var existing = CreateSampleWallet();
            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _walletRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var updateDto = new WalletCreateUpdateDto
            {
                DocumentId = "NEW123",
                Name = "Updated",
                Balance = 300
            };

            var result = await _service.UpdateAsync(1, updateDto);

            Assert.NotNull(result);
            Assert.Equal("NEW123", result!.DocumentId);
            Assert.Equal(300, result.Balance);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotExists()
        {
            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            var updateDto = new WalletCreateUpdateDto
            {
                DocumentId = "NEW123",
                Name = "Updated",
                Balance = 300
            };

            var result = await _service.UpdateAsync(1, updateDto);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesWallet_WhenExists()
        {
            var wallet = CreateSampleWallet();
            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(wallet);
            _walletRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
        {
            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            var result = await _service.DeleteAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task GetWithMovementsAsync_ReturnsDto_WhenExists()
        {
            var wallet = CreateSampleWallet();
            wallet.Movements.Add(new MovementHistory
            {
                Id = 1,
                Amount = 100,
                WalletId = wallet.Id,
                CreatedAt = DateTime.UtcNow,
                Type = Domain.Enums.MovementType.Credit
            });

            _walletRepoMock.Setup(r => r.GetByIdWithMovementsAsync(wallet.Id)).ReturnsAsync(wallet);

            var result = await _service.GetWithMovementsAsync(wallet.Id);

            Assert.NotNull(result);
            Assert.Single(result!.Movements);
        }

        [Fact]
        public async Task GetWithMovementsAsync_ReturnsNull_WhenNotFound()
        {
            _walletRepoMock.Setup(r => r.GetByIdWithMovementsAsync(99)).ReturnsAsync((Wallet?)null);

            var result = await _service.GetWithMovementsAsync(99);

            Assert.Null(result);
        }

        private Wallet CreateSampleWallet() => new Wallet
        {
            Id = 1,
            DocumentId = "123",
            Name = "Sample",
            Balance = 1000,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Movements = new List<MovementHistory>()
        };
    }
}