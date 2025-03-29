using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMovementHistoryRepository 
    {
        Task<MovementHistory?> GetByIdAsync(int id);
        Task<IEnumerable<MovementHistory>> GetAllAsync();
        Task<IEnumerable<MovementHistory>> GetByWalletIdAsync(int walletId);
        Task AddAsync(MovementHistory entity);
        void Update(MovementHistory entity);
        void Delete(MovementHistory entity);
        Task SaveChangesAsync();
    }
}
