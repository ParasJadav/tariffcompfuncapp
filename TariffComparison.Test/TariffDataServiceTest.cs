using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System.Collections.Generic;
using TariffComparison.Services;

namespace TariffComparison.Test
{
    [TestClass]
    public class TariffDataServiceTest
    {
        static TariffDataServiceTest()
        {
            // Correctly set the license context for EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("Paras POC");
        }
        [TestMethod]
        public void GetTariffData_Should_Return_Data_And_Cache_It()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var service = new TariffDataService();
            var sasUrl = Constants.SASUrl;

            // Act
            var result = service.GetTariffData(sasUrl, mockLogger.Object);

            // Assert
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.IsInstanceOfType(result, typeof(List<Dictionary<string, object>>), "Result should be a list of dictionaries.");
        }
    }
}
