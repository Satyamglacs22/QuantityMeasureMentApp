using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Controller;
using QuantityMeasurementAppModels.DTO;
using QuantityMeasurementAppRepository.Repositories;
using QuantityMeasurementAppService.Services;

namespace QuantityMeasurementApp.Tests.Integration
{
    [TestClass]
    public class QuantityMeasurementAppTest
    {
        private QuantityMeasurementController _controller;

        [TestInitialize]
        public void Setup()
        {
            var repo    = QuantityMeasurementCacheRepository.GetInstance();
            repo.DeleteAll();
            var service = new QuantityMeasurementServiceImpl(repo);
            _controller = new QuantityMeasurementController(service);
        }

        [TestMethod]
        public void TestLengthComparison()
        {
            var first  = new QuantityDTO(1.0, "Feet", "Length");
            var second = new QuantityDTO(12.0, "Inches", "Length");
            _controller.PerformLengthComparison(first, second);
        }

        [TestMethod]
        public void TestLengthConversion()
        {
            var quantity = new QuantityDTO(1.0, "Feet", "Length");
            _controller.PerformLengthConversion(quantity, "Inches");
        }

        [TestMethod]
        public void TestLengthAddition()
        {
            var first  = new QuantityDTO(1.0, "Feet", "Length");
            var second = new QuantityDTO(12.0, "Inches", "Length");
            _controller.PerformLengthAddition(first, second, "Feet");
        }

        [TestMethod]
        public void TestLengthSubtraction()
        {
            var first  = new QuantityDTO(2.0, "Feet", "Length");
            var second = new QuantityDTO(12.0, "Inches", "Length");
            _controller.PerformLengthSubtraction(first, second, "Feet");
        }
    }
}