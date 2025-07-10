using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TariffComparison.Helpers;

namespace TariffComparison.Services;

public class TariffDataService: ITariffDataService
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

    public List<Dictionary<string, object>> GetTariffData(string sasUrl, ILogger logger)
    {
        try
        {
            if (!Cache.TryGetValue(Constants.CacheKey, out List<Dictionary<string, object>>? cachedRows))
            {
                var result = ExcelHelper.ProcessExcelFromBlob(sasUrl, out var rows);
                if (result != null)
                {
                    logger.LogError("Error processing Excel from Blob: {Error}", result);
                    return new List<Dictionary<string, object>>();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                Cache.Set(Constants.CacheKey, rows, cacheEntryOptions);

                cachedRows = rows;
            }

            return cachedRows ?? new List<Dictionary<string, object>>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving tariff data.");
            return new List<Dictionary<string, object>>();
        }
    }
}