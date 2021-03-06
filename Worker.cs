using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IHttpClientFactory _httpClientFactory;

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var httpClient = _httpClientFactory.CreateClient(nameof(Worker));

                var response = await httpClient.GetAsync("api/v3/ticker/price");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    _logger.LogInformation($"ERROR CODE: {response.StatusCode}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
