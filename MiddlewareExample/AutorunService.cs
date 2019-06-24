using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MiddlewareExample
{
    public class AutorunService : IHostedService
    {
        private readonly IHttpClientFactory _clientFactory;
        private int _counter = 1;
        private Timer _timer;

        /// <inheritdoc />
        public AutorunService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(OnCheck, null, 10000, 10000);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void OnCheck(object state)
        {
            var currentJobId = _counter++;
            try
            {
                Console.WriteLine($"Starting job # {currentJobId}...");
                var client = _clientFactory.CreateClient();
                var result = await client.GetStringAsync("http://localhost:5000/api/values");
                Console.WriteLine($"Job # {currentJobId} completed successfully: {result}");
                Console.WriteLine("Success!!!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Job # {currentJobId} FAILED: {e}");
            }
        }
    }
}
