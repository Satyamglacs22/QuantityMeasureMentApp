using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModels.Entities;
using QuantityMeasurementAppRepositories.Context;
using QuantityMeasurementAppRepositories.Repositories;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityMeasurementRepositoryTests
    {
        private AppDbContext              _context = null!;
        private QuantityRecordRepository  _repo    = null!;

        [TestInitialize]
        public void Setup()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("RepoTestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);
            _repo    = new QuantityRecordRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void TestSave_And_GetAll_ReturnsRecord()
        {
            QuantityMeasurementEntity entity = new QuantityMeasurementEntity(
                "Add", 1.0, "Feet", 12.0, "Inch", 2.0, "Length");

            _repo.Save(entity);

            List<QuantityMeasurementEntity> all = _repo.GetAll();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("Add", all[0].Operation);
        }

        [TestMethod]
        public void TestGetByOperation_FiltersCorrectly()
        {
            _repo.Save(new QuantityMeasurementEntity("Add",     1.0, "Feet", 12.0, "Inch", 2.0,  "Length"));
            _repo.Save(new QuantityMeasurementEntity("Compare", 1.0, "Feet", 12.0, "Inch", 1.0,  "Length"));
            _repo.Save(new QuantityMeasurementEntity("Convert", 1.0, "Feet", 0.0,  "",     12.0, "Length"));

            List<QuantityMeasurementEntity> result = _repo.GetByOperation("Add");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Add", result[0].Operation);
        }

        [TestMethod]
        public void TestGetByMeasurementType_FiltersCorrectly()
        {
            _repo.Save(new QuantityMeasurementEntity("Add", 1.0, "Feet",     12.0, "Inch", 2.0, "Length"));
            _repo.Save(new QuantityMeasurementEntity("Add", 1.0, "Kilogram", 500.0, "Gram", 1.5, "Weight"));

            List<QuantityMeasurementEntity> result = _repo.GetByMeasurementType("Length");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Length", result[0].MeasurementType);
        }

        [TestMethod]
        public void TestGetErrorHistory_ReturnsOnlyErrors()
        {
            _repo.Save(new QuantityMeasurementEntity("Add",    1.0, "Feet", 12.0, "Inch", 2.0, "Length"));
            _repo.Save(new QuantityMeasurementEntity("Add",    "Some error message"));
            _repo.Save(new QuantityMeasurementEntity("Convert","Another error"));

            List<QuantityMeasurementEntity> errors = _repo.GetErrorHistory();
            Assert.AreEqual(2, errors.Count);
            Assert.IsTrue(errors.All(e => e.IsError));
        }

        [TestMethod]
        public void TestGetOperationCount_CountsOnlySuccessful()
        {
            _repo.Save(new QuantityMeasurementEntity("Add", 1.0, "Feet", 12.0, "Inch", 2.0, "Length"));
            _repo.Save(new QuantityMeasurementEntity("Add", 1.0, "Feet", 12.0, "Inch", 2.0, "Length"));
            _repo.Save(new QuantityMeasurementEntity("Add", "Error message"));

            int count = _repo.GetOperationCount("Add");
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void TestGetByCreatedAfter_FiltersCorrectly()
        {
            DateTime before = DateTime.UtcNow.AddSeconds(-1);

            _repo.Save(new QuantityMeasurementEntity("Add", 1.0, "Feet", 12.0, "Inch", 2.0, "Length"));

            List<QuantityMeasurementEntity> result = _repo.GetByCreatedAfter(before);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestGetAll_SortedByLatestFirst()
        {
            _repo.Save(new QuantityMeasurementEntity("Add",     1.0, "Feet", 0.0, "", 0.0, "Length"));
            _repo.Save(new QuantityMeasurementEntity("Compare", 2.0, "Feet", 0.0, "", 0.0, "Length"));

            List<QuantityMeasurementEntity> all = _repo.GetAll();
            Assert.AreEqual(2, all.Count);
            Assert.IsTrue(all[0].CreatedAt >= all[1].CreatedAt);
        }
    }
}
