using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;



namespace $safeprojectname$
{
    /// <summary>
    /// Please Run in PowerShell as Administrator after the creation of the .exe file 
    /// <code> sc.exe create "ServiceName" binpath="FullPath to the .exe file" </code>
    /// </summary>
    public class Worker : BackgroundService
    {
        private static readonly int ExecutionHour = 12;
        private readonly CronExpression _cron = CronExpression.Parse("0 " + ExecutionHour + " * * MON-FRI");

        private readonly ILogger<Worker> _Logger;
        private CancellationToken _cancellationToken;

        public Worker(ILogger<Worker> logger)
        {
            _Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _cancellationToken = stoppingToken;

            while (!_cancellationToken.IsCancellationRequested)
            {
                await ProductionMode(DateTime.UtcNow);
                //await TestMode(DateTime.UtcNow)
                await DelayTask();
            }
        }

        /// <summary>
        /// Set the system in TestMode
        /// </summary>
        /// <returns>Awaitable Task</returns>
        private async Task TestMode(DateTime Now)
        {
            await Task.Run(() => {
                // Insert Code Here For The Production Environment
                _Logger.LogInformation("Executed At: {utcNow}", Now);

            });
        }

        /// <summary>
        /// Set the system in ProductionMode
        /// </summary>
        /// <returns>Awaitable Task</returns>
        private async Task ProductionMode(DateTime Now)
        {
            //Execute only if the hour is between 
            if (Now.Hour >= ExecutionHour && Now.AddHours(1).Hour <= ExecutionHour)
            {
                await Task.Run(() => {
                    // Insert Code Here For The Production Environment

                });
            }
        }

        private Task DelayTask()
        {
            TimeSpan NextExecution = _cron.GetNextOccurrence(DateTime.UtcNow).Value - DateTime.UtcNow;
            _Logger.LogInformation("Next Execution in: {delay}", NextExecution);

            return Task.Delay(NextExecution);
        }

    }
}
