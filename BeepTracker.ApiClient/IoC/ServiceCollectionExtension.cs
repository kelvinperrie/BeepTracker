﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeepTracker.ApiClient.IoC
{
    public static class ServiceCollectionExtension
    {
        public static void AddClientService(this IServiceCollection services, Action<ClientOptions> options)
        {
            services.Configure(options);
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<ClientOptions>>().Value;
                var logger = provider.GetRequiredService<ILogger<ClientService>>();
                return new ClientService(options, logger);
            });
        }
    }
}
