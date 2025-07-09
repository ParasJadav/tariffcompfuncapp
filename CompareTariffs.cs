using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TariffComparison;

public class CompareTariffs
{
    private readonly ILogger<CompareTariffs> _logger;
    public CompareTariffs(ILogger<CompareTariffs> logger)
    {
        _logger = logger;
    }

    [Function("CompareTariffs")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
