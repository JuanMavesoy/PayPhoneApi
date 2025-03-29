using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infraestructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByIdWithMovementsAsync(int id)
        {
            return await _context.Wallets
                .Include(w => w.Movements)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task AddAsync(Wallet entity) => await _context.Wallets.AddAsync(entity);

        public void Delete(Wallet entity) => _context.Wallets.Remove(entity);

        public async Task<IEnumerable<Wallet>> GetAllAsync() => await _context.Wallets.ToListAsync();

        public async Task<Wallet?> GetByIdAsync(int id) => await _context.Wallets.FindAsync(id);

        public async Task<Wallet?> GetByDocumentIdAsync(string documentId) =>
            await _context.Wallets.FirstOrDefaultAsync(w => w.DocumentId == documentId);

        public void Update(Wallet entity) => _context.Wallets.Update(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}