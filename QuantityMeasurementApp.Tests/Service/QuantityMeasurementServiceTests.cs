using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppModels.DTO;
using QuantityMeasurementAppRepository.Repositories;
using QuantityMeasurementAppService.Services;

namespace QuantityMeasurementApp.Tests.Service
{
    [TestClass]
    public class QuantityMeasurementServiceTests
    {
        private QuantityMeasurementServiceImpl _service;

        [TestInitialize]
        public void Setup()
        {
            var repo = QuantityMeasurementCacheRepository.GetInstance();
            repo.DeleteAll();
            _service = new QuantityMeasurementServiceImpl(repo);
        }

        [TestMethod]
        public void TestCompare_EqualLengths()
        {
            var first  = new QuantityDTO(1.0, "Feet", "Length");
            var second = new QuantityDTO(12.0, "Inches", "Length");
            bool result = _service.Compare(first, second);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestAdd_TwoLengths()
        {
            var first  = new QuantityDTO(1.0, "Feet", "Length");
            var second = new QuantityDTO(12.0, "Inches", "Length");
            var result = _service.Add(first, second, "Feet");
            Assert.AreEqual(2.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestConvert_FeetToInches()
        {
            var quantity = new QuantityDTO(1.0, "Feet", "Length");
            var result   = _service.Convert(quantity, "Inches");
            Assert.AreEqual(12.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestSubtract_TwoWeights()
        {
            var first  = new QuantityDTO(2.0, "Kilogram", "Weight");
            var second = new QuantityDTO(1.0, "Kilogram", "Weight");
            var result = _service.Subtract(first, second, "Kilogram");
            Assert.AreEqual(1.0, result.Value, 0.01);
        }

        [TestMethod]
        public void TestDivide_TwoLengths()
        {
            var first  = new QuantityDTO(2.0, "Feet", "Length");
            var second = new QuantityDTO(1.0, "Feet", "Length");
            double result = _service.Divide(first, second);
            Assert.AreEqual(2.0, result, 0.01);
        }
    }
}