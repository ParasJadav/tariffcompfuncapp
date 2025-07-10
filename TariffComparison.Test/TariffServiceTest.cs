using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System.Collections.Generic;
using TariffComparison.Services;

namespace TariffComparison.Test
{
    [TestClass]
    public class TariffServiceTest
    {
        static TariffServiceTest()
        {
            // Correctly set the license context for EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("Paras POC");
        }

        [TestMethod]
        public void GetFilteredTariffs_Should_Filter_By_TariffType()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var service = new TariffService();
            var cachedRows = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { Constants.ProductName, "Basic Plan" },
                    { Constants.TariffType, Constants.TariffTypeBasic },
                    { Constants.BaseCost, 5 },
                    { Constants.IncludedKwh, 100 },
                    { Constants.AdditionalKwhCost, 0.2 }
                },
                new Dictionary<string, object>
                {
                    { Constants.ProductName, "Packaged Plan" },
                    { Constants.TariffType, Constants.TariffTypePackaged },
                    { Constants.BaseCost, 10 },
                    { Constants.IncludedKwh, 200 },
                    { Constants.AdditionalKwhCost, 0.15 }
                }
            };

            var requestData = new Dictionary<string, object>();
            var tarifftype = Constants.TariffTypeBasic;
            var consumption = "150";

            // Act
            var result = service.GetFilteredTariffs(cachedRows, tarifftype, consumption, requestData, mockLogger.Object);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual(1, result.Count, "Result should contain only one tariff.");
            Assert.AreEqual("Basic Plan", result[0].TariffName, "The tariff name should match the filtered type.");
        }

        [TestMethod]
        public void CalculateAnnualCost_Should_Calculate_Correctly_For_Basic_Tariff()
        {
            // Arrange
            var service = new TariffService();
            var row = new Dictionary<string, object>
            {
                { Constants.TariffType, Constants.TariffTypeBasic },
                { Constants.BaseCost, 5 },
                { Constants.AdditionalKwhCost, 0.2 }
            };
            var tarifftype = Constants.TariffTypeBasic;
            var consumption = "150";

            // Act
            var result = service.CalculateAnnualCost(row, tarifftype, consumption);

            // Assert
            Assert.AreEqual(90, result, "The annual cost for the basic tariff should be calculated correctly.");
        }

        [TestMethod]
        public void CalculateAnnualCost_Should_Calculate_Correctly_For_Packaged_Tariff()
        {
            // Arrange
            var service = new TariffService();
            var row = new Dictionary<string, object>
            {
                { Constants.TariffType, Constants.TariffTypePackaged },
                { Constants.BaseCost, 10 },
                { Constants.IncludedKwh, 200 },
                { Constants.AdditionalKwhCost, 0.15 }
            };
            var tarifftype = Constants.TariffTypePackaged;
            var consumption = "250";

            // Act
            var result = service.CalculateAnnualCost(row, tarifftype, consumption);

            // Assert
            Assert.AreEqual(17.5, result, "The annual cost for the packaged tariff should be calculated correctly.");
        }
    }
}
