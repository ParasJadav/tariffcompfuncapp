using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.Json;
using TariffComparison.Models;
using TariffComparison.Services;

namespace TariffComparison;

public class CompareTariffs
{
    private readonly ILogger<CompareTariffs> _logger;
    private readonly ITariffService _tariffService;
    private readonly ITariffDataService _tariffDataService;

    public CompareTariffs(ILogger<CompareTariffs> logger, ITariffService tariffService, ITariffDataService tariffDataService)
    {
        _logger = logger;
        _tariffService = tariffService;
        _tariffDataService = tariffDataService;
        ExcelPackage.License.SetNonCommercialPersonal("Paras POC");
    }

    [Function("CompareTariffs")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
    {
        try
        {
            _logger.LogInformation("C# HTTP trigger function CompareTariffs exectution started.");
            // Parse the request body
            string requestBody;
            var filteredResults = new List<TariffModel>();

            using (var streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }

            var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (requestData == null || !requestData.TryGetValue(Constants.RequestConsumption, out var consumptionObj) || !requestData.TryGetValue(Constants.RequestType, out var tarifftypeObj))
            {
                return new BadRequestObjectResult("Invalid request. Please provide 'consumption' and 'tarifftype'.");
            }

            var consumption = consumptionObj?.ToString();
            if (string.IsNullOrEmpty(consumption))
            {
                return new BadRequestObjectResult("Invalid 'consumption' value. It must be a number.");
            }

            var tarifftype = tarifftypeObj?.ToString();
            if (string.IsNullOrEmpty(tarifftype))
            {
                return new BadRequestObjectResult("Invalid 'tarifftype' value. It must be a non-empty string.");
            }

            var cachedRows = _tariffDataService.GetTariffData(Constants.SASUrl, _logger);

            if (cachedRows == null || cachedRows.Count == 0)
            {
                return new NotFoundObjectResult("No tariff data found.");
            }

            if (cachedRows != null && cachedRows.Count > 0)
            {
                filteredResults = _tariffService.GetFilteredTariffs(cachedRows, tarifftype, consumption, requestData, log);
            }

            _logger.LogInformation("C# HTTP trigger function CompareTariffs processed a request.");
            return new OkObjectResult(new { rows = filteredResults });
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Exception executing CompareTariffs function trigger." + ex.InnerException);
            return new NotFoundObjectResult("No tariff data found.");
        }
        
    }
}
