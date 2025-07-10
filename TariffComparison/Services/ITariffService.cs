using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TariffComparison.Models;

namespace TariffComparison.Services
{
    public interface ITariffService
    {
        List<TariffModel> GetFilteredTariffs(List<Dictionary<string, object>> cachedRows, string tarifftype, string consumption, Dictionary<string, object> requestData, ILogger logger);
    }
}
