using Microsoft.Extensions.Logging;
using TariffComparison.Models;

namespace TariffComparison.Services;

public interface ITariffService
{
    List<TariffModel> GetFilteredTariffs(
        List<Dictionary<string, object>> cachedRows,
        string tarifftype,
        string consumption,
        Dictionary<string, object> requestData,
        ILogger logger);

    double CalculateAnnualCost(Dictionary<string, object> row, string tarifftype, string consumption);
}