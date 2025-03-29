using Application.DTOs.MovementHistory;
using Application.Interfaces;
using Application.Mappers;
using Application.Utils;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;


namespace Application.Services
{
    public class MovementHistoryService : IMovementHistoryService
    {
        private readonly IMovementHistoryRepository _movementRepo;
        private readonly IWalletRepository _walletRepo;

        public MovementHistoryService(IMovementHistoryRepository movementRepo, IWalletRepository walletRepo)
        {
            _movementRepo = movementRepo;
            _walletRepo = walletRepo;
        }

        public async Task<IEnumerable<MovementHistoryDto>> GetAllAsync()
        {
            var movements = await _movementRepo.GetAllAsync();
            return movements.Select(MovementHistoryMapper.ToDto);
        }

        public async Task<IEnumerable<MovementHistoryDto>> GetByWalletIdAsync(int walletId)
        {
            var movements = await _movementRepo.GetByWalletIdAsync(walletId);
            return movements.Select(MovementHistoryMapper.ToDto);
        }

        public async Task<MovementHistoryDto> CreateAsync(MovementHistoryCreateDto dto)
        {
            if (!Enum.TryParse<MovementType>(dto.Type, true, out var type))
                throw new BusinessException("Tipo de movimiento inválido. Debe ser 'Credit' o 'Debit'.");

            var wallet = await _walletRepo.GetByIdAsync(dto.WalletId);
            if (wallet is null)
                throw new KeyNotFoundException($"No existe una billetera con ID {dto.WalletId}");

            var now = DateTimeHelper.GetColombiaTimeNow();

            var movement = new MovementHistory
            {
                WalletId = dto.WalletId,
                Amount = dto.Amount,
                Type = type,
                CreatedAt = now
            };

            await _movementRepo.AddAsync(movement);
            await _movementRepo.SaveChangesAsync();

            return MovementHistoryMapper.ToDto(movement);
        }

    }
}