using Microsoft.Extensions.Logging;
using TariffComparison.Models;

namespace TariffComparison.Services;

public class TariffService
{
    public List<TariffModel> GetFilteredTariffs(
    List<Dictionary<string, object>> cachedRows,
    string tarifftype,
    string consumption,
    Dictionary<string, object> requestData,
    ILogger logger)
    {
        var filteredResults = new List<TariffModel>();

        if (tarifftype.Equals(Constants.TariffTypeAll, StringComparison.OrdinalIgnoreCase))
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
                        logger.LogWarning("Invalid sortBy value provided. No sorting applied.");
                        break;
                }
            }
        }

        return filteredResults;
    }

    public double CalculateAnnualCost(Dictionary<string, object> row, string tarifftype, string consumption)
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