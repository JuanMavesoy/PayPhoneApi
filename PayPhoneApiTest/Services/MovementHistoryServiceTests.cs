using Application.DTOs.MovementHistory;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayPhoneApiTest.Services
{
    public class MovementHistoryServiceTests
    {
        private readonly Mock<IMovementHistoryRepository> _movementRepoMock;
        private readonly Mock<IWalletRepository> _walletRepoMock;
        private readonly MovementHistoryService _service;

        public MovementHistoryServiceTests()
        {
            _movementRepoMock = new Mock<IMovementHistoryRepository>();
            _walletRepoMock = new Mock<IWalletRepository>();
            _service = new MovementHistoryService(_movementRepoMock.Object, _walletRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfMovementHistoryDto()
        {
            var data = new List<MovementHistory>
            {
                new() { Id = 1, WalletId = 1, Amount = 100, Type = MovementType.Credit, CreatedAt = DateTime.UtcNow }
            };

            _movementRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

            var result = await _service.GetAllAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetByWalletIdAsync_ReturnsMovementsForWallet()
        {
            var walletId = 1;

            var data = new List<MovementHistory>
            {
                new() { Id = 1, WalletId = walletId, Amount = 50, Type = MovementType.Debit, CreatedAt = DateTime.UtcNow }
            };

            _movementRepoMock.Setup(r => r.GetByWalletIdAsync(walletId)).ReturnsAsync(data);

            var result = await _service.GetByWalletIdAsync(walletId);

            Assert.Single(result);
            Assert.Equal(walletId, result.First().WalletId);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenMovementTypeIsInvalid()
        {
            var dto = new MovementHistoryCreateDto
            {
                WalletId = 1,
                Amount = 50,
                Type = "Unknown"
            };

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CreateAsync(dto));

            Assert.Equal("Tipo de movimiento inválido. Debe ser 'Credit' o 'Debit'.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenWalletNotFound()
        {
            var dto = new MovementHistoryCreateDto
            {
                WalletId = 999,
                Amount = 50,
                Type = "Credit"
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(dto.WalletId)).ReturnsAsync((Wallet?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(dto));

            Assert.Equal("No existe una billetera con ID 999", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_SuccessfullyCreatesMovement()
        {
            var dto = new MovementHistoryCreateDto
            {
                WalletId = 1,
                Amount = 75,
                Type = "Debit"
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(dto.WalletId)).ReturnsAsync(new Wallet { Id = dto.WalletId });

            MovementHistory? captured = null;

            _movementRepoMock.Setup(r => r.AddAsync(It.IsAny<MovementHistory>()))
                .Callback<MovementHistory>(m => captured = m)
                .Returns(Task.CompletedTask);

            _movementRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.WalletId, result.WalletId);
            Assert.Equal(dto.Amount, result.Amount);
            Assert.Equal(MovementType.Debit.ToString(), result.Type);
            Assert.NotNull(captured);
            Assert.Equal(dto.Amount, captured!.Amount);
        }
    }
}