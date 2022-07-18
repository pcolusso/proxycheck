using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace proxycheck
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly CheckService _checkService;
        private readonly ILogger<WindowsBackgroundService> _logger;

        public WindowsBackgroundService(
            CheckService jokeService,
            ILogger<WindowsBackgroundService> logger) =>
            (_checkService, _logger) = (jokeService, logger);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(_checkService.GetProxyConf());

                string joke = await _checkService.CheckUrls();
                _logger.LogInformation(joke);

                _logger.LogInformation(_checkService.GetIEProxy());

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

