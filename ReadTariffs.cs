using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using TariffComparison.Helpers;

namespace TariffComparison;

public class ReadTariffs
{
    private readonly ILogger<CompareTariffs> _logger;
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    public ReadTariffs(ILogger<CompareTariffs> logger)
    {
        _logger = logger;

        // Correctly set the license context for EPPlus
        ExcelPackage.License.SetNonCommercialPersonal("Paras POC");

    }

    [Function("ReadTariffs")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
        ILogger log)
    {
        if (!Cache.TryGetValue(Constants.CacheKey, out List<Dictionary<string, object>>? cachedRows))
        {
            var sasUrl = Constants.SASUrl;

            var result = ExcelHelper.ProcessExcelFromBlob(sasUrl, out var rows);
            if (result != null)
            {
                return result;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            Cache.Set(Constants.CacheKey, rows, cacheEntryOptions);

            cachedRows = rows;
        }
        _logger.LogInformation("C# HTTP trigger function ReadTariffs processed a request.");
        return new OkObjectResult(new { rows = cachedRows });
    }
}
