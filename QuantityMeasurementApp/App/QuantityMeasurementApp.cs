using System;
using System.Collections.Generic;
using QuantityMeasurementApp.Controller;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Menu;
using QuantityMeasurementAppModels.Entity;
using QuantityMeasurementAppRepository.Interfaces;
using QuantityMeasurementAppRepository.Repositories;
using QuantityMeasurementAppRepository.Utilities;
using QuantityMeasurementAppService.Interfaces;
using QuantityMeasurementAppService.Services;

namespace QuantityMeasurementApp
{
    public class QuantityMeasurementApp
    {
        private static QuantityMeasurementApp? instance;
        private static readonly object lockObject = new object();

        private QuantityMeasurementController controller;
        private IMenu menu;
        private IQuantityMeasurementRepository repository;
        private string activeRepositoryType = string.Empty;

        private QuantityMeasurementApp()
        {
            Console.WriteLine("[App] Starting Quantity Measurement Application...");

            ApplicationConfig config = new ApplicationConfig();
            string repoType = config.GetRepositoryType();
            Console.WriteLine("[App] Configured repository type: " + repoType);

            if (repoType.Equals("database", StringComparison.OrdinalIgnoreCase))
            {
                TryInitializeDatabaseRepository(config);
            }
            else
            {
                Console.WriteLine("[App] Using Cache Repository.");
                InitializeCacheRepository();
            }

            IQuantityMeasurementService service = new QuantityMeasurementServiceImpl(repository);
            controller = new QuantityMeasurementController(service);
            menu       = new QuantityMenu(controller);

            Console.WriteLine("[App] Initialization complete.");
            Console.WriteLine("[App] Active repository: " + activeRepositoryType);
            Console.WriteLine("");
        }

        private void TryInitializeDatabaseRepository(ApplicationConfig config)
        {
            Console.WriteLine("[App] Attempting to connect to SQL Server...");
            try
            {
                ConnectionPool pool = ConnectionPool.GetInstance(config);
                repository           = new QuantityMeasurementDatabaseRepository(pool);
                activeRepositoryType = "Database (SQL Server)";
                Console.WriteLine("[App] SQL Server connected. Using Database Repository.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[App] WARNING: SQL Server not available: " + ex.Message);
                Console.WriteLine("[App] Switching to Cache Repository...");
                InitializeCacheRepository();
            }
        }

        private void InitializeCacheRepository()
        {
            repository           = QuantityMeasurementCacheRepository.GetInstance();
            activeRepositoryType = "Cache (data saved to JSON file)";
            Console.WriteLine("[App] Cache Repository ready.");
        }

        public static QuantityMeasurementApp GetInstance()
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                        instance = new QuantityMeasurementApp();
                }
            }
            return instance;
        }

        public void Start()
        {
            menu.Run();
        }

        public void ReportAllMeasurements()
        {
            Console.WriteLine("\n========== Measurement History ==========");
            Console.WriteLine("Repository : " + activeRepositoryType);

            List<QuantityMeasurementEntity> all = repository.GetAll();
            Console.WriteLine("Total records: " + all.Count);

            for (int i = 0; i < all.Count; i++)
                Console.WriteLine((i + 1) + ". " + all[i].ToString());

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Pool stats : " + repository.GetPoolStatistics());
            Console.WriteLine("=========================================\n");
        }

        public void DeleteAllMeasurements()
        {
            Console.WriteLine("[App] Deleting all measurements...");
            repository.DeleteAll();
            Console.WriteLine("[App] Done. Count now: " + repository.GetTotalCount());
        }

        public void CloseResources()
        {
            Console.WriteLine("[App] Closing resources...");
            repository.ReleaseResources();
            Console.WriteLine("[App] Resources closed.");
        }
    }
}