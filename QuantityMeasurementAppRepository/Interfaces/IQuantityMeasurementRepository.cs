using System.Collections.Generic;
using QuantityMeasurementAppModels.Entity;

namespace QuantityMeasurementAppRepository.Interfaces
{
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity entity);
        List<QuantityMeasurementEntity> GetAll();
        List<QuantityMeasurementEntity> GetByOperation(string operation);
        List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);
        int GetTotalCount();
        void DeleteAll();
        string GetPoolStatistics();
        void ReleaseResources();
    }
}