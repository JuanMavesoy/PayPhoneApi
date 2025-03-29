using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories
{
    public class MovementHistoryRepository : IMovementHistoryRepository
    {
        private readonly AppDbContext _context;

        public MovementHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MovementHistory entity) => await _context.MovementsHistory.AddAsync(entity);

        public void Delete(MovementHistory entity) => _context.MovementsHistory.Remove(entity);

        public async Task<IEnumerable<MovementHistory>> GetAllAsync() => await _context.MovementsHistory.ToListAsync();

        public async Task<MovementHistory?> GetByIdAsync(int id) => await _context.MovementsHistory.FindAsync(id);

        public async Task<IEnumerable<MovementHistory>> GetByWalletIdAsync(int walletId) =>
            await _context.MovementsHistory.Where(m => m.WalletId == walletId).ToListAsync();

        public void Update(MovementHistory entity) => _context.MovementsHistory.Update(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}