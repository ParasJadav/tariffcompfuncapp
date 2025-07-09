using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

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
        const string cacheKey = "PlanData";

        if (!Cache.TryGetValue(cacheKey, out List<Dictionary<string, object>>? cachedRows))
        {
            // Use the provided SAS URL
            var sasUrl = "https://tariffcomparisonstorage.blob.core.windows.net/plan/Plan.xlsx?sp=r&st=2025-07-09T10:51:53Z&se=2025-07-09T18:51:53Z&spr=https&sv=2024-11-04&sr=b&sig=FNUoJCzpLdV%2BqbYb52Ze9Dy91heqI6Kh0Ml%2FeE%2B%2BXrQ%3D";

            var blobClient = new BlobClient(new Uri(sasUrl));

            if (!blobClient.Exists())
            {
                return new NotFoundObjectResult("Blob not found at the provided SAS URL.");
            }

            using var memoryStream = new MemoryStream();
            blobClient.DownloadTo(memoryStream);
            memoryStream.Position = 0;

            using var package = new ExcelPackage(memoryStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                return new BadRequestObjectResult("No worksheet found in the Excel file.");
            }

            var rows = new List<Dictionary<string, object>>();
            var columnNames = new List<string>();

            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                columnNames.Add(worksheet.Cells[1, col].Text);
            }

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowData = new Dictionary<string, object>();
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    rowData[columnNames[col - 1]] = worksheet.Cells[row, col].Text;
                }
                rows.Add(rowData);
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            Cache.Set(cacheKey, rows, cacheEntryOptions);

            cachedRows = rows;
        }
        _logger.LogInformation("C# HTTP trigger function ReadTariffs processed a request.");
        return new OkObjectResult(new { rows = cachedRows });
    }
}
