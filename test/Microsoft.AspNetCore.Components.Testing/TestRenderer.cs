﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components.Testing
{
    [SuppressMessage("Usage", "BL0006:Do not use RenderTree types", Justification = "<Pending>")]
    internal class TestRenderer : Renderer
    {
        private Exception _unhandledException;

        private TaskCompletionSource<object> _nextRenderTcs = new TaskCompletionSource<object>();

        public TestRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
            : base(serviceProvider, loggerFactory)
        {
            ServiceProvider = serviceProvider;
        }

        public new ArrayRange<RenderTreeFrame> GetCurrentRenderTreeFrames(int componentId)
            => base.GetCurrentRenderTreeFrames(componentId);

        public int AttachTestRootComponent(ContainerComponent testRootComponent)
            => AssignRootComponentId(testRootComponent);

        public new Task DispatchEventAsync(ulong eventHandlerId, EventFieldInfo fieldInfo, EventArgs eventArgs)
        {
            var task = Dispatcher.InvokeAsync(
                () => base.DispatchEventAsync(eventHandlerId, fieldInfo, eventArgs));
            AssertNoSynchronousErrors();
            return task;
        }

        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

        public Task NextRender => _nextRenderTcs.Task;

        public IServiceProvider ServiceProvider { get; }

        protected override void HandleException(Exception exception)
        {
            _unhandledException = exception;
        }
        public void DispatchAndAssertNoSynchronousErrors(Action callback)
        {
            Dispatcher.InvokeAsync(callback).Wait();
            AssertNoSynchronousErrors();
        }

        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            // TODO: Capture batches (and the state of component output) for individual inspection
            var prevTcs = _nextRenderTcs;
            _nextRenderTcs = new TaskCompletionSource<object>();
            prevTcs.SetResult(null);
            return Task.CompletedTask;
        }

        private void AssertNoSynchronousErrors()
        {
            if (_unhandledException != null)
            {
                ExceptionDispatchInfo.Capture(_unhandledException).Throw();
            }
        }
    }
}
