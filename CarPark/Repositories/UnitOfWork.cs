using CarPark.Data;
using CarPark.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarPark.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
            => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction.");
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync();
        }

        public Task<int> SaveChangesAsync()
            => _context.SaveChangesAsync();

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}