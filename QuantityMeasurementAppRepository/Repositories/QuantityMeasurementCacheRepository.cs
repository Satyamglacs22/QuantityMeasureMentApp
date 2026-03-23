using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using QuantityMeasurementAppModels.Entity;
using QuantityMeasurementAppRepository.Interfaces;

namespace QuantityMeasurementAppRepository.Repositories
{
    // UC16 changes:
    // 1. Fixed hardcoded file path - now uses AppDomain.CurrentDomain.BaseDirectory
    // 2. Implemented all 6 new interface methods (GetByOperation, GetByMeasurementType,
    //    GetTotalCount, DeleteAll, GetPoolStatistics, ReleaseResources)

    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private static QuantityMeasurementCacheRepository instance;
        private static readonly object lockObject = new object();

        private List<QuantityMeasurementEntity> history;

        private static readonly string filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "QuantityMeasurement.json");

        private QuantityMeasurementCacheRepository()
        {
            history = new List<QuantityMeasurementEntity>();
            LoadFromDisk();
            Console.WriteLine("[CacheRepository] Initialized. File: " + filePath);
        }

        // Singleton - thread safe
        public static QuantityMeasurementCacheRepository GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new QuantityMeasurementCacheRepository();
                    }
                }
            }
            return instance;
        }

        // UC15 - Save entity to in-memory list and persist to JSON file
        public void Save(QuantityMeasurementEntity entity)
        {
            lock (lockObject)
            {
                history.Add(entity);
                SaveToDisk();
            }
        }

        // UC15 - Return a copy of all stored entities
        public List<QuantityMeasurementEntity> GetAll()
        {
            return new List<QuantityMeasurementEntity>(history);
        }

        // UC16 - Filter by operation name (case-insensitive)
        public List<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            List<QuantityMeasurementEntity> result = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in history)
            {
                if (entity.Operation != null
                    && entity.Operation.Equals(
                        operation, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        // UC16 - Filter by measurement type (case-insensitive)
        public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
        {
            List<QuantityMeasurementEntity> result = new List<QuantityMeasurementEntity>();

            foreach (QuantityMeasurementEntity entity in history)
            {
                if (entity.MeasurementType != null
                    && entity.MeasurementType.Equals(
                        measurementType, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        // UC16 - Return count of all stored entities
        public int GetTotalCount()
        {
            return history.Count;
        }

        // UC16 - Clear all entities from memory and disk
        public void DeleteAll()
        {
            lock (lockObject)
            {
                history.Clear();
                SaveToDisk();
                Console.WriteLine("[CacheRepository] All measurements deleted.");
            }
        }

        // UC16 - No connection pool for cache; return informational message
        public string GetPoolStatistics()
        {
            return "In-memory cache. No connection pool. Records: " + history.Count;
        }

        // UC16 - No resources to release for cache
        public void ReleaseResources()
        {
            Console.WriteLine("[CacheRepository] No resources to release.");
        }

        // Persists the in-memory list to disk as JSON
        private void SaveToDisk()
        {
            try
            {
                string json = JsonSerializer.Serialize(history);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CacheRepository] Warning: could not save to disk: "
                    + ex.Message);
            }
        }

        // Loads previous session data from the JSON file on startup
        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    List<QuantityMeasurementEntity> loaded =
                        JsonSerializer.Deserialize<List<QuantityMeasurementEntity>>(json);

                    if (loaded != null)
                    {
                        history = loaded;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[CacheRepository] Warning: could not load from disk: "
                    + ex.Message);
                history = new List<QuantityMeasurementEntity>();
            }
        }
    }
}