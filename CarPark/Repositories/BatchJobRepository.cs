using CarPark.Data;
using CarPark.Interfaces;
using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Repositories
{
    public class BatchJobRepository : IBatchJobRepository
    {
        private readonly AppDbContext _context;

        public BatchJobRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<BatchJobRecord?> GetByFileNameAsync(string fileName)
            => _context.BatchJobRecords
                .OrderByDescending(b => b.StartedAt)
                .FirstOrDefaultAsync(b => b.FileName == fileName);

        public async Task AddAsync(BatchJobRecord record)
            => await _context.BatchJobRecords.AddAsync(record);

        public Task UpdateAsync(BatchJobRecord record)
        {
            _context.BatchJobRecords.Update(record);
            return Task.CompletedTask;
        }
    }
}