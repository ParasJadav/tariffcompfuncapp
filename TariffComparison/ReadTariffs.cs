using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using TariffComparison.Services;

namespace TariffComparison;

public class ReadTariffs
{
    private readonly ILogger<CompareTariffs> _logger;
    private readonly TariffDataService _tariffDataService;

    public ReadTariffs(ILogger<CompareTariffs> logger)
    {
        _logger = logger;
        _tariffDataService = new TariffDataService();

        // Correctly set the license context for EPPlus
        ExcelPackage.License.SetNonCommercialPersonal("Paras POC");
    }

    [Function("ReadTariffs")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
        ILogger log)
    {
        var cachedRows = _tariffDataService.GetTariffData(Constants.SASUrl, _logger);

        if (cachedRows == null || cachedRows.Count == 0)
        {
            return new NotFoundObjectResult("No tariff data found.");
        }

        _logger.LogInformation("C# HTTP trigger function ReadTariffs processed a request.");
        return new OkObjectResult(new { rows = cachedRows });
    }
}
