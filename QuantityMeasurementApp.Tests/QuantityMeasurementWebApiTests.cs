using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementAppModels.DTOs;
using QuantityMeasurementAppRepositories.Context;

namespace QuantityMeasurementWebApi.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class QuantityMeasurementWebApiTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _client = null!;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BASE_URL = "/api/v1/quantities";

        // -------------------------------------------------------
        // Setup and Teardown
        // -------------------------------------------------------

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace SQL Server with InMemory database for tests
                        ServiceDescriptor? descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<AppDbContext>(options =>
                            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
                    });
                });

            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        // -------------------------------------------------------
        // Helper Methods
        // -------------------------------------------------------

        private static StringContent ToJson(object obj)
        {
            string json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static QuantityDTO MakeQuantity(double value, string unit, string type)
        {
            return new QuantityDTO { Value = value, UnitName = unit, MeasurementType = type };
        }

        // -------------------------------------------------------
        // Startup Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestWebApiApplicationStarts()
        {
            HttpResponseMessage response = await _client.GetAsync("/health");

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task TestServerIsReachable()
        {
            HttpResponseMessage response = await _client.GetAsync("/swagger/index.html");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // -------------------------------------------------------
        // Compare Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestCompare_EqualLengths_ReturnsTrue()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet",  "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual("True", result.ResultString);
            Assert.AreEqual("Compare", result.Operation);
        }

        [TestMethod]
        public async Task TestCompare_UnequalLengths_ReturnsFalse()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(5.0, "Inch", "Length")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual("False", result.ResultString);
        }

        [TestMethod]
        public async Task TestCompare_EqualWeights_ReturnsTrue()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,    "Kilogram", "Weight"),
                ThatQuantityDTO = MakeQuantity(1000.0, "Gram",     "Weight")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual("True", result.ResultString);
        }

        [TestMethod]
        public async Task TestCompare_EqualVolumes_ReturnsTrue()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,    "Litre",      "Volume"),
                ThatQuantityDTO = MakeQuantity(1000.0, "Millilitre", "Volume")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual("True", result.ResultString);
        }

        [TestMethod]
        public async Task TestCompare_EqualTemperatures_ReturnsTrue()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(100.0, "Celsius",    "Temperature"),
                ThatQuantityDTO = MakeQuantity(212.0, "Fahrenheit", "Temperature")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual("True", result.ResultString);
        }

        // -------------------------------------------------------
        // Convert Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestConvert_FeetToInch_Returns12()
        {
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet", "Length"),
                TargetUnit      = "Inch"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/convert", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(12.0, result.ResultValue, 0.01);
            Assert.AreEqual("Convert", result.Operation);
        }

        [TestMethod]
        public async Task TestConvert_KilogramToGram_Returns1000()
        {
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Kilogram", "Weight"),
                TargetUnit      = "Gram"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/convert", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1000.0, result.ResultValue, 0.01);
        }

        [TestMethod]
        public async Task TestConvert_LitreToMillilitre_Returns1000()
        {
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Litre", "Volume"),
                TargetUnit      = "Millilitre"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/convert", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1000.0, result.ResultValue, 0.01);
        }

        [TestMethod]
        public async Task TestConvert_CelsiusToFahrenheit_Returns212()
        {
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(100.0, "Celsius", "Temperature"),
                TargetUnit      = "Fahrenheit"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/convert", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(212.0, result.ResultValue, 0.01);
        }

        // -------------------------------------------------------
        // Add Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestAdd_OneFeetPlusTwelveInch_Returns2Feet()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,  "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length"),
                TargetUnit      = "Feet"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(2.0, result.ResultValue, 0.01);
            Assert.AreEqual("Add", result.Operation);
        }

        [TestMethod]
        public async Task TestAdd_TwoWeights_ReturnsCorrectSum()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,   "Kilogram", "Weight"),
                ThatQuantityDTO = MakeQuantity(500.0, "Gram",     "Weight"),
                TargetUnit      = "Kilogram"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1.5, result.ResultValue, 0.01);
        }

        [TestMethod]
        public async Task TestAdd_TwoVolumes_ReturnsCorrectSum()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,    "Litre",      "Volume"),
                ThatQuantityDTO = MakeQuantity(500.0,  "Millilitre", "Volume"),
                TargetUnit      = "Litre"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1.5, result.ResultValue, 0.01);
        }

        // -------------------------------------------------------
        // Subtract Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestSubtract_TwoFeetMinusOnefoot_ReturnsOneFoot()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(2.0,  "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length"),
                TargetUnit      = "Feet"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/subtract", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1.0, result.ResultValue, 0.01);
            Assert.AreEqual("Subtract", result.Operation);
        }

        [TestMethod]
        public async Task TestSubtract_TwoWeights_ReturnsCorrectDiff()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(2.0, "Kilogram", "Weight"),
                ThatQuantityDTO = MakeQuantity(1.0, "Kilogram", "Weight"),
                TargetUnit      = "Kilogram"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/subtract", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1.0, result.ResultValue, 0.01);
        }

        // -------------------------------------------------------
        // Divide Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestDivide_TwoFeetByOneFoot_ReturnsTwo()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(2.0, "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(1.0, "Feet", "Length")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/divide", ToJson(request));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            QuantityMeasurementResponseDTO? result =
                JsonSerializer.Deserialize<QuantityMeasurementResponseDTO>(body, _jsonOptions);

            Assert.IsNotNull(result);
            Assert.AreEqual(2.0, result.ResultValue, 0.01);
            Assert.AreEqual("Divide", result.Operation);
        }

        [TestMethod]
        public async Task TestDivide_ByZero_Returns500()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(0.0, "Feet", "Length")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/divide", ToJson(request));

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        // -------------------------------------------------------
        // History and Count Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestGetHistoryByOperation_ReturnsRecords()
        {
            // First perform a compare
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,  "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length")
            };
            await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            // Then get history
            HttpResponseMessage response = await _client.GetAsync(BASE_URL + "/history/operation/Compare");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            List<QuantityMeasurementResponseDTO>? results =
                JsonSerializer.Deserialize<List<QuantityMeasurementResponseDTO>>(body, _jsonOptions);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count >= 1);
        }

        [TestMethod]
        public async Task TestGetHistoryByType_ReturnsRecords()
        {
            // Perform a convert to ensure a record exists
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Kilogram", "Weight"),
                TargetUnit      = "Gram"
            };
            await _client.PostAsync(BASE_URL + "/convert", ToJson(request));

            HttpResponseMessage response = await _client.GetAsync(BASE_URL + "/history/type/Weight");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            List<QuantityMeasurementResponseDTO>? results =
                JsonSerializer.Deserialize<List<QuantityMeasurementResponseDTO>>(body, _jsonOptions);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count >= 1);
        }

        [TestMethod]
        public async Task TestGetErrorHistory_ReturnsErrorRecords()
        {
            // Trigger an error — mismatched types
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet",     "Length"),
                ThatQuantityDTO = MakeQuantity(1.0, "Kilogram", "Weight"),
                TargetUnit      = "Feet"
            };
            await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            HttpResponseMessage response = await _client.GetAsync(BASE_URL + "/history/errored");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            List<QuantityMeasurementResponseDTO>? results =
                JsonSerializer.Deserialize<List<QuantityMeasurementResponseDTO>>(body, _jsonOptions);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count >= 1);
            Assert.IsTrue(results[0].IsError);
        }

        [TestMethod]
        public async Task TestGetOperationCount_ReturnsCount()
        {
            // Perform an add
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,  "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length"),
                TargetUnit      = "Feet"
            };
            await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            HttpResponseMessage response = await _client.GetAsync(BASE_URL + "/count/Add");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            int count = int.Parse(body);
            Assert.IsTrue(count >= 1);
        }

        // -------------------------------------------------------
        // Validation / Error Handling Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestCompare_InvalidUnit_Returns400()
        {
            QuantityInputRequest request = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,  "Feet",  "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "INCHE", "Length")
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", ToJson(request));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task TestAdd_MismatchedTypes_Returns400()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Feet",     "Length"),
                ThatQuantityDTO = MakeQuantity(1.0, "Kilogram", "Weight"),
                TargetUnit      = "Feet"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task TestCompare_MissingBody_Returns400()
        {
            StringContent emptyBody = new StringContent("{}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/compare", emptyBody);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task TestTemperatureArithmetic_Returns400()
        {
            ArithmeticRequest request = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(100.0, "Celsius", "Temperature"),
                ThatQuantityDTO = MakeQuantity(50.0,  "Celsius", "Temperature"),
                TargetUnit      = "Celsius"
            };

            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/add", ToJson(request));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // -------------------------------------------------------
        // Swagger and Config Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestSwaggerDocumentation_ContainsEndpoints()
        {
            HttpResponseMessage response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            string body = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(body.Contains("compare"));
            Assert.IsTrue(body.Contains("convert"));
            Assert.IsTrue(body.Contains("add"));
            Assert.IsTrue(body.Contains("subtract"));
            Assert.IsTrue(body.Contains("divide"));
            Assert.IsTrue(body.Contains("200"));
            Assert.IsTrue(body.Contains("400"));
        }

        [TestMethod]
        public async Task TestConfig_Development_LogLevelIsDebug()
        {
            IServiceScope scope = _factory.Services.CreateScope();
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            string? logLevel = config["Logging:LogLevel:Default"];
            Assert.AreEqual("Debug", logLevel);
        }

        [TestMethod]
        public async Task TestConfig_Production_OverrideWorks()
        {
            WebApplicationFactory<Program> prodFactory =
                _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((ctx, cfg) =>
                    {
                        cfg.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "Logging:LogLevel:Default", "Warning" }
                        });
                    });
                });

            HttpClient client = prodFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync("/health");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            IServiceScope scope = prodFactory.Services.CreateScope();
            IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            string? logLevel = config["Logging:LogLevel:Default"];
            Assert.AreEqual("Warning", logLevel);

            client.Dispose();
            prodFactory.Dispose();
        }

        // -------------------------------------------------------
        // Security Placeholder Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestSecurity_WithoutAuth_HealthStillReachable()
        {
            HttpResponseMessage response = await _client.GetAsync("/health");
            Assert.IsNotNull(response);
            Assert.IsTrue(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task TestSecurity_WithAuth_HealthReturns200()
        {
            HttpResponseMessage response = await _client.GetAsync("/health");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        // -------------------------------------------------------
        // Multi-Step Integration Tests
        // -------------------------------------------------------

        [TestMethod]
        public async Task TestMultipleOperationsFlow()
        {
            // Step 1 — Compare
            QuantityInputRequest compareRequest = new QuantityInputRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,  "Feet", "Length"),
                ThatQuantityDTO = MakeQuantity(12.0, "Inch", "Length")
            };
            HttpResponseMessage compareResponse =
                await _client.PostAsync(BASE_URL + "/compare", ToJson(compareRequest));
            Assert.AreEqual(HttpStatusCode.OK, compareResponse.StatusCode);

            // Step 2 — Convert
            ConvertRequest convertRequest = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(3.0, "Feet", "Length"),
                TargetUnit      = "Yard"
            };
            HttpResponseMessage convertResponse =
                await _client.PostAsync(BASE_URL + "/convert", ToJson(convertRequest));
            Assert.AreEqual(HttpStatusCode.OK, convertResponse.StatusCode);

            // Step 3 — Add weights
            ArithmeticRequest addRequest = new ArithmeticRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0,   "Kilogram", "Weight"),
                ThatQuantityDTO = MakeQuantity(500.0, "Gram",     "Weight"),
                TargetUnit      = "Kilogram"
            };
            HttpResponseMessage addResponse =
                await _client.PostAsync(BASE_URL + "/add", ToJson(addRequest));
            Assert.AreEqual(HttpStatusCode.OK, addResponse.StatusCode);

            // Step 4 — Verify count
            HttpResponseMessage countResponse = await _client.GetAsync(BASE_URL + "/count/Add");
            string countBody = await countResponse.Content.ReadAsStringAsync();
            int count = int.Parse(countBody);
            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public async Task TestDatabaseInitialization_PersistsData()
        {
            // Perform a convert
            ConvertRequest request = new ConvertRequest
            {
                ThisQuantityDTO = MakeQuantity(1.0, "Litre", "Volume"),
                TargetUnit      = "Millilitre"
            };
            HttpResponseMessage response = await _client.PostAsync(BASE_URL + "/convert", ToJson(request));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify it appears in history
            HttpResponseMessage historyResponse =
                await _client.GetAsync(BASE_URL + "/history/operation/Convert");
            string body = await historyResponse.Content.ReadAsStringAsync();
            List<QuantityMeasurementResponseDTO>? results =
                JsonSerializer.Deserialize<List<QuantityMeasurementResponseDTO>>(body, _jsonOptions);

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count >= 1);
            Assert.IsNotNull(results[0].Operation);
            Assert.IsNotNull(results[0].ThisUnit);
        }
    }
}
