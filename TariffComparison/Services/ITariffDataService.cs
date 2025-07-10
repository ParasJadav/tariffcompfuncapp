using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TariffComparison.Services
{
    public interface ITariffDataService
    {
        List<Dictionary<string, object>> GetTariffData(string sasUrl, ILogger logger);
    }
}
