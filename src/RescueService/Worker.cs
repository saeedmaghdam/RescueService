using Microsoft.Extensions.Options;

namespace RescueService
{
    public class Worker : BackgroundService
    {
        private const string ACTIVE = "active";

        private readonly ILogger<Worker> _logger;
        private readonly IOptionsMonitor<ApplicationOptions> _options;
        private readonly InfraService _infraService;

        public Worker(ILogger<Worker> logger, IOptionsMonitor<ApplicationOptions> options, InfraService infraService)
        {
            _logger = logger;
            _options = options;
            _infraService = infraService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                await RescueServicesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_options.CurrentValue.DelayInSeconds), stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task RescueServicesAsync(CancellationToken stoppingToken)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _options.CurrentValue.MaxDegreeOfParallelism };
            await Parallel.ForEachAsync(_options.CurrentValue.SystemDemandOptions, options, async (systemDemandOption, CancellationToken) => { await RescueServiceAsync(systemDemandOption, stoppingToken); });
        }

        private async Task RescueServiceAsync(SystemDemandOption systemDemandOption, CancellationToken cancellationToken)
        {
            var commandString = CommandFactory.SystemCtlStatusCommand(systemDemandOption.ServiceName);
            var serviceStatus = await _infraService.ExecuteCommandAsync(systemDemandOption.HostOrIpAddress, systemDemandOption.Port, systemDemandOption.Username, systemDemandOption.Password, commandString, TimeSpan.FromSeconds(5), cancellationToken);

            if (serviceStatus == ACTIVE)
            {
                _logger.LogInformation($"{systemDemandOption.ServiceTitle} is active.");
                return;
            }
            else
            {
                _logger.LogWarning($"{systemDemandOption.ServiceTitle} is not active, trying to restart the service.");

                commandString = CommandFactory.SystemCtlRestartCommand(systemDemandOption.ServiceName);
                _ = await _infraService.ExecuteCommandAsync(systemDemandOption.HostOrIpAddress, systemDemandOption.Port, systemDemandOption.Username, systemDemandOption.Password, commandString, TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}