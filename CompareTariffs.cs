using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Text.Json;
using TariffComparison.Helpers;
using TariffComparison.Models;

namespace TariffComparison;

public class CompareTariffs
{
    private readonly ILogger<CompareTariffs> _logger;
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
    public CompareTariffs(ILogger<CompareTariffs> logger)
    {
        _logger = logger;
        ExcelPackage.License.SetNonCommercialPersonal("Paras POC");
    }

    [Function("CompareTariffs")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
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

        if (!Cache.TryGetValue(Constants.CacheKey, out List<Dictionary<string, object>>? cachedRows))
        {
            var sasUrl = Constants.SASUrl;

            ExcelHelper.ProcessExcelFromBlob(sasUrl, out var rows);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };
            Cache.Set(Constants.CacheKey, rows, cacheEntryOptions);

            cachedRows = rows;
        }

        if (cachedRows != null && cachedRows.Count > 0)
        {
            if (tarifftype.Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                filteredResults = cachedRows.Select(row => new TariffModel
                {
                    TariffName = row.TryGetValue(Constants.ProductName, out var nameValue) && nameValue != null
                        ? nameValue.ToString()
                        : string.Empty,
                    TariffType = row.TryGetValue(Constants.TariffType, out var typeValue) && typeValue != null
                        ? (typeValue.ToString() == Constants.TariffTypeBasic ? Constants.TariffTypeBasicName : Constants.TariffTypePackagedName)
                        : string.Empty,
                    AnnualCost = CalculateAnnualCost(row, tarifftype, consumption)
                }).ToList();
            }
            else if (!string.IsNullOrEmpty(tarifftype))
            {
                filteredResults = cachedRows.Where(row => row[Constants.TariffType]?.ToString() == tarifftype).Select(row => new TariffModel
                {
                    TariffName = row.TryGetValue(Constants.ProductName, out var nameValue) && nameValue != null
                        ? nameValue.ToString()
                        : string.Empty,
                    TariffType = row.TryGetValue(Constants.TariffType, out var typeValue) && typeValue != null
                        ? (typeValue.ToString() == Constants.TariffTypeBasic ? Constants.TariffTypeBasicName : Constants.TariffTypePackagedName)
                        : string.Empty,
                    AnnualCost = CalculateAnnualCost(row, tarifftype, consumption)
                }).ToList();

                if (requestData.TryGetValue(Constants.RequestSortBy, out var sortBy))
                {
                    var sortByValue = sortBy?.ToString();
                    switch (sortByValue?.ToLower())
                    {
                        case "priceasc":
                            filteredResults = filteredResults.OrderBy(t => t.AnnualCost).ToList();
                            break;
                        case "pricedesc":
                            filteredResults = filteredResults.OrderByDescending(t => t.AnnualCost).ToList();
                            break;
                        case "nameasc":
                            filteredResults = filteredResults.OrderBy(t => t.TariffName).ToList();
                            break;
                        case "namedesc":
                            filteredResults = filteredResults.OrderByDescending(t => t.TariffName).ToList();
                            break;
                        case "typeasc":
                            filteredResults = filteredResults.OrderBy(t => t.TariffType).ToList();
                            break;
                        case "typedesc":
                            filteredResults = filteredResults.OrderByDescending(t => t.TariffType).ToList();
                            break;
                        default:
                            _logger.LogWarning("Invalid sortBy value provided. No sorting applied.");
                            break;
                    }
                    
                }
            }
            else if (filteredResults.Count == 0)
            {
                return new NotFoundObjectResult("No tariffs found matching the specified criteria.");
            }
        }

        _logger.LogInformation("C# HTTP trigger function CompareTariffs processed a request.");
        return new OkObjectResult(new { rows = filteredResults });

    }

    double CalculateAnnualCost(Dictionary<string, object> row, string tarifftype, string consumption)
    {
        double basicCost = 0.00;
        double consumptionCost = 0.00;

        if (!string.IsNullOrEmpty(consumption))
        {
            if (tarifftype.Equals(Constants.TariffTypeAll))
            {
                if (row.TryGetValue(Constants.TariffType, out var type))
                {
                    if (!string.IsNullOrEmpty(type.ToString()))
                    {
                        if (type.ToString().Equals(Constants.TariffTypeBasic, StringComparison.OrdinalIgnoreCase))
                        {
                            if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                            {
                                basicCost = Convert.ToDouble(baseCost) * 12;
                            }
                            if (row.TryGetValue(Constants.AdditionalKwhCost, out var additionalKwhCost))
                            {
                                consumptionCost = Convert.ToDouble(additionalKwhCost) * Convert.ToDouble(consumption);
                            }
                        }
                        else if (type.ToString().Equals(Constants.TariffTypePackaged, StringComparison.OrdinalIgnoreCase))
                        {
                            if (row.TryGetValue(Constants.IncludedKwh, out var includedKwh))
                            {
                                if (Convert.ToDouble(includedKwh) >= Convert.ToDouble(consumption))
                                {
                                    if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                                    {
                                        basicCost = Convert.ToDouble(baseCost);
                                    }
                                }
                                else
                                {
                                    if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                                    {
                                        basicCost = Convert.ToDouble(baseCost);
                                    }
                                    double difference = Convert.ToDouble(consumption) - Convert.ToDouble(includedKwh);
                                    if (row.TryGetValue(Constants.AdditionalKwhCost, out var additionalKwhCost))
                                    {
                                        consumptionCost = Convert.ToDouble(additionalKwhCost) * difference;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (tarifftype.Equals(Constants.TariffTypeBasic))
            {
                if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                {
                    basicCost = Convert.ToDouble(baseCost) * 12;
                }
                if (row.TryGetValue(Constants.AdditionalKwhCost, out var additionalKwhCost))
                {
                    consumptionCost = Convert.ToDouble(consumption) * Convert.ToDouble(additionalKwhCost);
                }
            }
            else if (tarifftype.Equals(Constants.TariffTypePackaged))
            {
                if (row.TryGetValue(Constants.IncludedKwh, out var includedKwh))
                {
                    if (Convert.ToDouble(includedKwh) >= Convert.ToDouble(consumption))
                    {
                        if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                        {
                            basicCost = Convert.ToDouble(baseCost);
                        }
                    }
                    else
                    {
                        if (row.TryGetValue(Constants.BaseCost, out var baseCost))
                        {
                            basicCost = Convert.ToDouble(baseCost);
                        }
                        double difference = Convert.ToDouble(consumption) - Convert.ToDouble(includedKwh);
                        if (row.TryGetValue(Constants.AdditionalKwhCost, out var additionalKwhCost))
                        {
                            consumptionCost = Convert.ToDouble(additionalKwhCost) * difference;
                        }
                    }
                }
            }
            else
            {
                return 0.00;
            }
        }


        return basicCost + consumptionCost;
    }
}
