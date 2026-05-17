using CarPark.Batch;
using CarPark.Interfaces;
using CarPark.Models;

namespace CarPark.Services
{
    public interface IBatchImportService
    {
        Task ImportAsync(string filePath);
    }

    public class BatchImportService : IBatchImportService
    {
        private readonly CsvCarParkParser _parser;
        private readonly ICarParkRepository _carParkRepo;
        private readonly IBatchJobRepository _batchJobRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BatchImportService> _logger;

        public BatchImportService(
            CsvCarParkParser parser,
            ICarParkRepository carParkRepo,
            IBatchJobRepository batchJobRepo,
            IUnitOfWork uow,
            ILogger<BatchImportService> logger)
        {
            _parser = parser;
            _carParkRepo = carParkRepo;
            _batchJobRepo = batchJobRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task ImportAsync(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            _logger.LogInformation("Starting batch import: {FileName}", fileName);

            // Skip if already successfully processed
            var existing = await _batchJobRepo.GetByFileNameAsync(fileName);
            if (existing?.Status == BatchJobStatus.Completed)
            {
                _logger.LogWarning("File {FileName} already processed. Skipping.", fileName);
                return;
            }

            // Create or reuse job record
            var jobRecord = existing ?? new BatchJobRecord
            {
                FileName = fileName,
                StartedAt = DateTime.UtcNow
            };

            jobRecord.Status = BatchJobStatus.Running;
            jobRecord.StartedAt = DateTime.UtcNow;
            jobRecord.ErrorMessage = null;

            if (existing is null)
                await _batchJobRepo.AddAsync(jobRecord);
            else
                await _batchJobRepo.UpdateAsync(jobRecord);

            // Save job start record outside main transaction
            await _uow.SaveChangesAsync();

            try
            {
                await _uow.BeginTransactionAsync();

                var carParks = await _parser.ParseAsync(filePath);
                jobRecord.TotalRows = carParks.Count;

                _logger.LogInformation("Parsed {Count} records.", carParks.Count);

                await _carParkRepo.UpsertRangeAsync(carParks);
                jobRecord.ProcessedRows = carParks.Count;

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                jobRecord.Status = BatchJobStatus.Completed;
                jobRecord.CompletedAt = DateTime.UtcNow;

                _logger.LogInformation("Import completed. {Count} records upserted.", carParks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import failed for {FileName}. Rolling back.", fileName);

                await _uow.RollbackAsync();

                jobRecord.Status = BatchJobStatus.Failed;
                jobRecord.ErrorMessage = ex.Message;
                jobRecord.CompletedAt = DateTime.UtcNow;
            }
            finally
            {
                // Always update job record with final status
                await _batchJobRepo.UpdateAsync(jobRecord);
                await _uow.SaveChangesAsync();
            }
        }
    }
}