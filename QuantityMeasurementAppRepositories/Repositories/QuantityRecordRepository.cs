using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Context;
using QuantityMeasurementAppRepositories.Interfaces;

namespace QuantityMeasurementAppRepositories.Repositories
{
    public class QuantityRecordRepository : IQuantityRecordRepository
    {
        private readonly AppDbContext context;

        public QuantityRecordRepository(AppDbContext context)
        {
            this.context = context;
        }

        // Save entity to database
        public void Save(QuantityMeasurementEntity entity)
        {
            context.QuantityMeasurements.Add(entity);
            context.SaveChanges();
        }

        // Get all records sorted by latest first
        public List<QuantityMeasurementEntity> GetAll()
        {
            return context.QuantityMeasurements
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        // Get records by operation type
        public List<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            return context.QuantityMeasurements
                .Where(e => e.Operation == operation)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        // Get records by measurement type
        public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
        {
            return context.QuantityMeasurements
                .Where(e => e.MeasurementType == measurementType)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        // Get error records only
        public List<QuantityMeasurementEntity> GetErrorHistory()
        {
            return context.QuantityMeasurements
                .Where(e => e.IsError == true)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        // Get count of successful operations by type
        public int GetOperationCount(string operation)
        {
            return context.QuantityMeasurements
                .Count(e => e.Operation == operation && e.IsError == false);
        }

        // Get records created after a specific date
        public List<QuantityMeasurementEntity> GetByCreatedAfter(DateTime date)
        {
            return context.QuantityMeasurements
                .Where(e => e.CreatedAt > date)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }
    }
}
