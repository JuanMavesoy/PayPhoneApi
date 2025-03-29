using Application.DTOs.Wallet;

namespace Application.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletDto>> GetAllAsync();
        Task<WalletDto?> GetByIdAsync(int id);
        Task<WalletDto> CreateAsync(WalletCreateUpdateDto walletDto);
        Task<WalletDto?> UpdateAsync(int id, WalletCreateUpdateDto walletDto);
        Task<bool> DeleteAsync(int id);
        Task<WalletWithMovementsDto?> GetWithMovementsAsync(int id);
    }
}
