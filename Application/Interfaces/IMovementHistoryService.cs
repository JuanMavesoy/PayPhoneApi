using Application.DTOs.MovementHistory;


namespace Application.Interfaces
{
    public interface IMovementHistoryService
    {
        Task<IEnumerable<MovementHistoryDto>> GetAllAsync();
        Task<IEnumerable<MovementHistoryDto>> GetByWalletIdAsync(int walletId);
        Task<MovementHistoryDto> CreateAsync(MovementHistoryCreateDto dto);

    }
}
