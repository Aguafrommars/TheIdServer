using Microsoft.Extensions.Logging;
using SQLitePCL;
using System;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    class TestLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly bool _isEnable;

        public TestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _isEnable = Environment.GetEnvironmentVariable("APPVEYOR") == null;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
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
