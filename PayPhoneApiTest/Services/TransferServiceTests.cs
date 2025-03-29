using Application.DTOs.Transfer;
using Application.Services;
using Domain.Entities;
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
    public class TransferServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepoMock;
        private readonly Mock<IMovementHistoryRepository> _movementRepoMock;
        private readonly TransferService _service;

        public TransferServiceTests()
        {
            _walletRepoMock = new Mock<IWalletRepository>();
            _movementRepoMock = new Mock<IMovementHistoryRepository>();
            _service = new TransferService(_walletRepoMock.Object, _movementRepoMock.Object);
        }

        [Fact]
        public async Task TransferAsync_Throws_WhenWalletsAreEqual()
        {
            var dto = new TransferRequestDto
            {
                FromWalletId = 1,
                ToWalletId = 1,
                Amount = 100
            };

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.TransferAsync(dto));
            Assert.Equal("La billetera origen y destino no pueden ser la misma.", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_Throws_WhenFromWalletNotFound()
        {
            var dto = new TransferRequestDto
            {
                FromWalletId = 1,
                ToWalletId = 2,
                Amount = 100
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Wallet?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.TransferAsync(dto));
            Assert.Equal("No existe la billetera origen con ID 1", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_Throws_WhenToWalletNotFound()
        {
            var fromWallet = CreateWallet(1, 200);

            var dto = new TransferRequestDto
            {
                FromWalletId = 1,
                ToWalletId = 2,
                Amount = 50
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fromWallet);
            _walletRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Wallet?)null);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.TransferAsync(dto));
            Assert.Equal("No existe la billetera destino con ID 2", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_Throws_WhenInsufficientBalance()
        {
            var fromWallet = CreateWallet(1, 30);
            var toWallet = CreateWallet(2, 100);

            var dto = new TransferRequestDto
            {
                FromWalletId = 1,
                ToWalletId = 2,
                Amount = 50
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fromWallet);
            _walletRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(toWallet);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.TransferAsync(dto));
            Assert.Equal("La billetera origen no tiene saldo suficiente para la transferencia.", ex.Message);
        }

        [Fact]
        public async Task TransferAsync_SuccessfullyTransfersFunds()
        {
            var fromWallet = CreateWallet(1, 200);
            var toWallet = CreateWallet(2, 100);

            var dto = new TransferRequestDto
            {
                FromWalletId = 1,
                ToWalletId = 2,
                Amount = 50
            };

            _walletRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fromWallet);
            _walletRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(toWallet);

            _movementRepoMock.Setup(r => r.AddAsync(It.IsAny<MovementHistory>())).Returns(Task.CompletedTask);
            _walletRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _movementRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _service.TransferAsync(dto);

            Assert.Equal(150, fromWallet.Balance);
            Assert.Equal(150, toWallet.Balance);

            _walletRepoMock.Verify(r => r.Update(fromWallet), Times.Once);
            _walletRepoMock.Verify(r => r.Update(toWallet), Times.Once);
            _movementRepoMock.Verify(r => r.AddAsync(It.IsAny<MovementHistory>()), Times.Exactly(2));
        }

        private Wallet CreateWallet(int id, decimal balance) => new Wallet
        {
            Id = id,
            DocumentId = $"DOC{id}",
            Name = $"Wallet{id}",
            Balance = balance,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}