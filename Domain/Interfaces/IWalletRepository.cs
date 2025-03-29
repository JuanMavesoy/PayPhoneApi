using Domain.Entities;


namespace Domain.Interfaces
{
    public interface IWalletRepository 
    {
        Task<Wallet?> GetByIdWithMovementsAsync(int id);
        Task<Wallet?> GetByIdAsync(int id);
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet?> GetByDocumentIdAsync(string documentId);
        Task AddAsync(Wallet entity);
        void Update(Wallet entity);
        void Delete(Wallet entity);
        Task SaveChangesAsync();
    }
}
