using QuantityMeasurementAppModels.Entities;

namespace QuantityMeasurementAppRepositories.Interfaces
{
    public interface IQuantityRecordRepository
    {
        void Save(QuantityMeasurementEntity entity);
        List<QuantityMeasurementEntity> GetAll();
        List<QuantityMeasurementEntity> GetByOperation(string operation);
        List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);
        List<QuantityMeasurementEntity> GetErrorHistory();
        int GetOperationCount(string operation);
        List<QuantityMeasurementEntity> GetByCreatedAfter(DateTime date);
    }
}
