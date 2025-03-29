using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Application.Utils;
using Application.Mappers;
using Application.DTOs.Wallet;

namespace Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<WalletWithMovementsDto?> GetWithMovementsAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdWithMovementsAsync(id);
            return wallet is null ? null : WalletMapper.ToDtoWithMovements(wallet);
        }

        public async Task<IEnumerable<WalletDto>> GetAllAsync()
        {
            var wallets = await _walletRepository.GetAllAsync();
            return wallets.Select(WalletMapper.ToDto);
        }

        public async Task<WalletDto?> GetByIdAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            return wallet is null ? null : WalletMapper.ToDto(wallet);
        }

        public async Task<WalletDto> CreateAsync(WalletCreateUpdateDto walletDto)
        {
            var now = DateTimeHelper.GetColombiaTimeNow();

            var wallet = new Wallet
            {
                DocumentId = walletDto.DocumentId,
                Name = walletDto.Name,
                Balance = walletDto.Balance,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _walletRepository.AddAsync(wallet);
            await _walletRepository.SaveChangesAsync();

            return WalletMapper.ToDto(wallet);
        }

        public async Task<WalletDto?> UpdateAsync(int id, WalletCreateUpdateDto walletDto)
        {
            var now = DateTimeHelper.GetColombiaTimeNow();

            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet is null) return null;

            wallet.Name = walletDto.Name;
            wallet.DocumentId = walletDto.DocumentId;
            wallet.Balance = walletDto.Balance;
            wallet.UpdatedAt = now;

            _walletRepository.Update(wallet);
            await _walletRepository.SaveChangesAsync();

            return WalletMapper.ToDto(wallet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet is null) return false;

            _walletRepository.Delete(wallet);
            await _walletRepository.SaveChangesAsync();

            return true;
        }
    }
}