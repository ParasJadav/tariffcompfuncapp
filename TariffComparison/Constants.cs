using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffComparison
{
    public class Constants
    {
        public const string TariffTypeAll = "0";
        public const string TariffTypeBasic = "1";
        public const string TariffTypePackaged = "2";
        public const string TariffTypeBasicName = "Basic";
        public const string TariffTypePackagedName = "Packaged";
        public const string TariffType = "type";
        public const string ProductName = "name";
        public const string BaseCost = "baseCost";
        public const string IncludedKwh = "includedKwh";
        public const string AdditionalKwhCost = "additionalKwhCost";
        public const string CacheKey = "PlanData"; // Cache key for storing plan data
        public const string RequestType = "tarifftype";
        public const string RequestConsumption = "consumption";
        public const string RequestSortBy = "sortby";
        public const string SASUrl = "https://tariffcomparisonstorage.blob.core.windows.net/plan/Plan.xlsx?sp=r&st=2025-07-10T06:14:45Z&se=2025-07-31T14:14:45Z&spr=https&sv=2024-11-04&sr=b&sig=S8UBkJjY9JupnONORArhuLExP6kuU7KTRmy9ejpkr%2F4%3D";
    }
}
