using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;

namespace Microsoft.AspNetCore.Components.Testing
{
    public class TestHost
    {
        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        private readonly Lazy<TestRenderer> _renderer;

        public IServiceProvider ServiceProvider => Renderer.ServiceProvider;

        public TestHost()
        {
            _renderer = new Lazy<TestRenderer>(() =>
            {
                var serviceProvider = _serviceCollection.BuildServiceProvider();
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? new NullLoggerFactory();
                return new TestRenderer(serviceProvider, loggerFactory);
            });
        }

        public void AddService<T>(T implementation)
            => AddService<T, T>(implementation);

        public void AddService<TContract, TImplementation>(TImplementation implementation) where TImplementation: TContract
        {
            if (_renderer.IsValueCreated)
            {
                throw new InvalidOperationException("Cannot configure services after the host has started operation");
            }

            _serviceCollection.AddSingleton(typeof(TContract), implementation);
        }

        public void ConfigureServices(Action<IServiceCollection> configure)
        {
            configure(_serviceCollection);
        }

        public void WaitForNextRender(Action trigger = null)
        {
            var task = Renderer.NextRender;
            trigger?.Invoke();
            if (Debugger.IsAttached)
            {
                task.Wait();
            }
            else
            {
                task.Wait(millisecondsTimeout: 5000);
            }
            

            if (!task.IsCompleted)
            {
                throw new TimeoutException("No render occurred within the timeout period.");
            }
        }

        public RenderedComponent<TComponent> AddComponent<TComponent>(ParameterView? parameterView = null) where TComponent : IComponent
        {
            var result = new RenderedComponent<TComponent>(Renderer);
            result.SetParametersAndRender(parameterView ?? ParameterView.Empty);
            return result;
        }

        private TestRenderer Renderer => _renderer.Value;
    }
}
