using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffComparison
{
    internal class Constants
    {
        internal const string TariffTypeAll = "0";
        internal const string TariffTypeBasic = "1";
        internal const string TariffTypePackaged = "2";
        internal const string TariffTypeBasicName = "Basic";
        internal const string TariffTypePackagedName = "Packaged";
        internal const string TariffType = "type";
        internal const string ProductName = "name";
        internal const string BaseCost = "baseCost";
        internal const string IncludedKwh = "includedKwh";
        internal const string AdditionalKwhCost = "additionalKwhCost";
        internal const string CacheKey = "PlanData"; // Cache key for storing plan data
        internal const string RequestType = "tarifftype";
        internal const string RequestConsumption = "consumption";
        internal const string RequestSortBy = "sortby";
        internal const string SASUrl = "https://tariffcomparisonstorage.blob.core.windows.net/plan/Plan.xlsx?sp=r&st=2025-07-09T10:51:53Z&se=2025-07-09T18:51:53Z&spr=https&sv=2024-11-04&sr=b&sig=FNUoJCzpLdV%2BqbYb52Ze9Dy91heqI6Kh0Ml%2FeE%2B%2BXrQ%3D";
    }
}
