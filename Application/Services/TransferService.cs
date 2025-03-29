using Application.DTOs.Transfer;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IMovementHistoryRepository _movementRepo;

        public TransferService(IWalletRepository walletRepo, IMovementHistoryRepository movementRepo)
        {
            _walletRepo = walletRepo;
            _movementRepo = movementRepo;
        }

        public async Task TransferAsync(TransferRequestDto dto)
        {
            if (dto.FromWalletId == dto.ToWalletId)
                throw new BusinessException("La billetera origen y destino no pueden ser la misma.");

            var fromWallet = await _walletRepo.GetByIdAsync(dto.FromWalletId);
            var toWallet = await _walletRepo.GetByIdAsync(dto.ToWalletId);

            if (fromWallet is null)
                throw new KeyNotFoundException($"No existe la billetera origen con ID {dto.FromWalletId}");

            if (toWallet is null)
                throw new KeyNotFoundException($"No existe la billetera destino con ID {dto.ToWalletId}");

            if (fromWallet.Balance < dto.Amount)
                throw new BusinessException("La billetera origen no tiene saldo suficiente para la transferencia.");

            var now = DateTimeHelper.GetColombiaTimeNow();


            fromWallet.Balance -= dto.Amount;
            toWallet.Balance += dto.Amount;

            fromWallet.UpdatedAt = now;
            toWallet.UpdatedAt = now;

            _walletRepo.Update(fromWallet);
            _walletRepo.Update(toWallet);

       
            var debit = new MovementHistory
            {
                WalletId = fromWallet.Id,
                Amount = dto.Amount,
                Type = MovementType.Debit,
                CreatedAt = now
            };

            var credit = new MovementHistory
            {
                WalletId = toWallet.Id,
                Amount = dto.Amount,
                Type = MovementType.Credit,
                CreatedAt = now
            };

            await _movementRepo.AddAsync(debit);
            await _movementRepo.AddAsync(credit);

            await _walletRepo.SaveChangesAsync();
            await _movementRepo.SaveChangesAsync();
        }
    }
}