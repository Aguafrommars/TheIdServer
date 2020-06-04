using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Testing
{
    public class TestHost : IDisposable
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
                task.Wait(5000);
            }
            

            if (!task.IsCompleted)
            {
                throw new TimeoutException("No render occurred within the timeout period.");
            }
        }

        public void WaitForNoRender()
        {
            var task = Renderer.NextRender;
            while(task.Wait(500))
            {
                task = Renderer.NextRender;
            }
        }

        public async Task WaitForNextRenderAsync(Func<Task> trigger = null)
        {
            var task = Renderer.NextRender;
            await trigger?.Invoke();
            if (Debugger.IsAttached)
            {
                task.Wait();
            }
            else
            {
                task.Wait(5000);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _renderer.IsValueCreated)
                {
#pragma warning disable BL0006 // Do not use RenderTree types
                    try
                    {
                        _renderer.Value.Dispose();
                    }
                    catch(InvalidOperationException)
                    {
                        // silent
                    }
#pragma warning restore BL0006 // Do not use RenderTree types
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
