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
        public const string SASUrl = "https://tariffcomparisonstorage.blob.core.windows.net/plan/Plan.xlsx?sp=r&st=2025-08-10T08:02:33Z&se=2025-08-30T16:17:33Z&spr=https&sv=2024-11-04&sr=b&sig=GeJHKt0MmbK8BcBgGblikhVcJ6HUvqhInCqP8Y5L%2B0s%3D";
    }
}
