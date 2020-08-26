// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    class TestLogger : ILogger, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly bool _isEnable;

        public TestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _isEnable = Environment.GetEnvironmentVariable("APPVEYOR") != null;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
            // nothing to dispose
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel > LogLevel.Information)
            {
                return true;
            }
            return _isEnable;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            try
            {
                _testOutputHelper.WriteLine(formatter(state, exception));
            }
            catch
            {
                // silent
            }
        }
    }
}
