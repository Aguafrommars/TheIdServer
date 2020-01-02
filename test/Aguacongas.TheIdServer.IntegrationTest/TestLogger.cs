using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    class TestLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public TestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;    
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {            
            _testOutputHelper.WriteLine(formatter(state, exception));
        }
    }
}
