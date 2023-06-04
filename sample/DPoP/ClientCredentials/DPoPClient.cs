// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClientCredentialsDPoPClient;

public class DPoPClient : BackgroundService
{
    private readonly ILogger<DPoPClient> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public DPoPClient(ILogger<DPoPClient> logger, IHttpClientFactory factory)
    {
        _logger = logger;
        _clientFactory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(2000, stoppingToken);
            
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("\n\n");
            _logger.LogInformation("DPoPClient running at: {time}", DateTimeOffset.Now);

            var client = _clientFactory.CreateClient("client");
            var response = await client.GetAsync("identity", stoppingToken);
                
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                _logger.LogInformation("API response: {response}", content);    
            }
            else
            {
                _logger.LogError("API returned: {statusCode}", response.StatusCode);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}