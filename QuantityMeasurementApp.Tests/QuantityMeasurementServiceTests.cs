using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppRepositories.Context;
using QuantityMeasurementAppRepositories.Repositories;
using QuantityMeasurementAppServices.Services;

namespace QuantityMeasurementApp.Tests
{
    [TestClass]
    public class QuantityMeasurementServiceTests
    {
        private QuantityMeasurementServiceImpl _service = null!;
        private QuantityRecordRepository       _repo    = null!;
        private AppDbContext                   _context = null!;

        [TestInitialize]
        public void Setup()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("ServiceTestDb_" + Guid.NewGuid())
                .Options;

            _context = new AppDbContext(options);
            _repo    = new QuantityRecordRepository(_context);
            _service = new QuantityMeasurementServiceImpl(_repo);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // -------------------------------------------------------
        // Compare Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestCompare_OneFeetTwelveInch_ReturnsTrue()
        {
            QuantityDTO first  = new QuantityDTO(1.0,  "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");

            bool result = _service.Compare(first, second);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompare_OneKilogramOneThousandGram_ReturnsTrue()
        {
            QuantityDTO first  = new QuantityDTO(1.0,    "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(1000.0, "Gram",     "Weight");

            bool result = _service.Compare(first, second);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompare_OneLitreOneThousandMl_ReturnsTrue()
        {
            QuantityDTO first  = new QuantityDTO(1.0,    "Litre",      "Volume");
            QuantityDTO second = new QuantityDTO(1000.0, "Millilitre", "Volume");

            bool result = _service.Compare(first, second);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompare_100Celsius212Fahrenheit_ReturnsTrue()
        {
            QuantityDTO first  = new QuantityDTO(100.0, "Celsius",    "Temperature");
            QuantityDTO second = new QuantityDTO(212.0, "Fahrenheit", "Temperature");

            bool result = _service.Compare(first, second);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestCompare_DifferentLengths_ReturnsFalse()
        {
            QuantityDTO first  = new QuantityDTO(1.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(5.0, "Inch", "Length");

            bool result = _service.Compare(first, second);

            Assert.IsFalse(result);
        }

        // -------------------------------------------------------
        // Convert Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestConvert_FeetToInch_Returns12()
        {
            QuantityDTO quantity = new QuantityDTO(1.0, "Feet", "Length");

            QuantityDTO result = _service.Convert(quantity, "Inch");

            Assert.AreEqual(12.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestConvert_KilogramToGram_Returns1000()
        {
            QuantityDTO quantity = new QuantityDTO(1.0, "Kilogram", "Weight");

            QuantityDTO result = _service.Convert(quantity, "Gram");

            Assert.AreEqual(1000.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestConvert_LitreToMillilitre_Returns1000()
        {
            QuantityDTO quantity = new QuantityDTO(1.0, "Litre", "Volume");

            QuantityDTO result = _service.Convert(quantity, "Millilitre");

            Assert.AreEqual(1000.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestConvert_CelsiusToFahrenheit_Returns212()
        {
            QuantityDTO quantity = new QuantityDTO(100.0, "Celsius", "Temperature");

            QuantityDTO result = _service.Convert(quantity, "Fahrenheit");

            Assert.AreEqual(212.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestConvert_CelsiusToKelvin_Returns373()
        {
            QuantityDTO quantity = new QuantityDTO(100.0, "Celsius", "Temperature");

            QuantityDTO result = _service.Convert(quantity, "Kelvin");

            Assert.AreEqual(373.15, result.Value, 0.01);
        }

        // -------------------------------------------------------
        // Add Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestAdd_OneFeetPlusTwelveInch_Returns2Feet()
        {
            QuantityDTO first  = new QuantityDTO(1.0,  "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");

            QuantityDTO result = _service.Add(first, second, "Feet");

            Assert.AreEqual(2.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestAdd_OneKgPlusFiveHundredGram_Returns1Point5Kg()
        {
            QuantityDTO first  = new QuantityDTO(1.0,   "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(500.0, "Gram",     "Weight");

            QuantityDTO result = _service.Add(first, second, "Kilogram");

            Assert.AreEqual(1.5, result.Value, 0.01);
        }

        [TestMethod]
        public void TestAdd_OneLitrePlusFiveHundredMl_Returns1Point5Litre()
        {
            QuantityDTO first  = new QuantityDTO(1.0,   "Litre",      "Volume");
            QuantityDTO second = new QuantityDTO(500.0, "Millilitre", "Volume");

            QuantityDTO result = _service.Add(first, second, "Litre");

            Assert.AreEqual(1.5, result.Value, 0.01);
        }

        // -------------------------------------------------------
        // Subtract Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestSubtract_TwoFeetMinusTwelveInch_ReturnsOneFoot()
        {
            QuantityDTO first  = new QuantityDTO(2.0,  "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");

            QuantityDTO result = _service.Subtract(first, second, "Feet");

            Assert.AreEqual(1.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestSubtract_TwoKgMinusOneKg_ReturnsOneKg()
        {
            QuantityDTO first  = new QuantityDTO(2.0, "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(1.0, "Kilogram", "Weight");

            QuantityDTO result = _service.Subtract(first, second, "Kilogram");

            Assert.AreEqual(1.0, result.Value, 0.01);
        }

        // -------------------------------------------------------
        // Divide Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestDivide_TwoFeetByOneFoot_ReturnsTwo()
        {
            QuantityDTO first  = new QuantityDTO(2.0, "Feet", "Length");
            QuantityDTO second = new QuantityDTO(1.0, "Feet", "Length");

            double result = _service.Divide(first, second);

            Assert.AreEqual(2.0, result, 0.01);
        }

        [TestMethod]
        public void TestDivide_TwoKgByOneKg_ReturnsTwo()
        {
            QuantityDTO first  = new QuantityDTO(2.0, "Kilogram", "Weight");
            QuantityDTO second = new QuantityDTO(1.0, "Kilogram", "Weight");

            double result = _service.Divide(first, second);

            Assert.AreEqual(2.0, result, 0.01);
        }

        // -------------------------------------------------------
        // Repository Persistence Tests
        // -------------------------------------------------------

        [TestMethod]
        public void TestCompare_SavesRecordToRepository()
        {
            QuantityDTO first  = new QuantityDTO(1.0,  "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");

            _service.Compare(first, second);

            int count = _repo.GetOperationCount("Compare");
            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public void TestConvert_SavesRecordToRepository()
        {
            QuantityDTO quantity = new QuantityDTO(1.0, "Feet", "Length");

            _service.Convert(quantity, "Inch");

            int count = _repo.GetOperationCount("Convert");
            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public void TestAdd_SavesRecordToRepository()
        {
            QuantityDTO first  = new QuantityDTO(1.0,  "Feet", "Length");
            QuantityDTO second = new QuantityDTO(12.0, "Inch", "Length");

            _service.Add(first, second, "Feet");

            int count = _repo.GetOperationCount("Add");
            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public void TestError_SavesErrorRecordToRepository()
        {
            QuantityDTO first  = new QuantityDTO(1.0, "Feet",     "Length");
            QuantityDTO second = new QuantityDTO(1.0, "Kilogram", "Weight");

            try { _service.Add(first, second, "Feet"); }
            catch { }

            var errors = _repo.GetErrorHistory();
            Assert.IsTrue(errors.Count >= 1);
            Assert.IsTrue(errors[0].IsError);
        }
    }
}
