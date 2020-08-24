using System;
using System.Threading.Tasks;
using MariCommands.Features;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Features
{
    public class FeaturesTests
    {

        [Fact]
        public async ValueTask DisposablesWillDisposeIDisposable()
        {
            var disposed = false;
            var disposables = new DisposablesFeature();

            var disposable = new TestDisposableClass(() =>
            {
                disposed = true;
            });

            disposables.RegisterForDispose(disposable);
            await disposables.DisposeAsync();

            Assert.True(disposed);
        }

        [Fact]
        public async ValueTask DisposablesWillDisposeIAsyncDisposable()
        {
            var disposed = false;
            var disposables = new DisposablesFeature();

            var asyncDisposable = new TestAsyncDisposableClass(() =>
            {
                disposed = true;
            });

            disposables.RegisterForDisposeAsync(asyncDisposable);
            await disposables.DisposeAsync();

            Assert.True(disposed);
        }

        [Fact]
        public async ValueTask DisposeDisposablesWillClearRegisteredDisposables()
        {
            var disposedCount = 0;
            var disposables = new DisposablesFeature();

            var disposable = new TestDisposableClass(() =>
            {
                disposedCount++;
            });

            disposables.RegisterForDispose(disposable);

            // call two times.
            await disposables.DisposeAsync();
            await disposables.DisposeAsync();

            Assert.Equal(1, disposedCount);
        }

        [Fact]
        public async ValueTask DisposeDisposablesWillClearRegisteredAsyncDisposables()
        {
            var disposedCount = 0;
            var disposables = new DisposablesFeature();

            var asyncDisposable = new TestAsyncDisposableClass(() =>
            {
                disposedCount++;
            });

            disposables.RegisterForDisposeAsync(asyncDisposable);

            // call two times.
            await disposables.DisposeAsync();
            await disposables.DisposeAsync();

            Assert.Equal(1, disposedCount);
        }

        [Fact]
        public void ThrowsExceptionIfWithoutServiceScopeFactory()
        {
            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, null);

            Assert.Throws<InvalidOperationException>(() =>
            {
                commandsServices.CommandServices.GetService<TestDisposableClass>();
            });
        }

        [Fact]
        public void CanGetScopedDependencyCommandServices()
        {
            var provider = new ServiceCollection()
                                .AddScoped<TestScopedDependency>()
                                .BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, scopeFactory);

            var scopedDependency = commandsServices.CommandServices.GetRequiredService<TestScopedDependency>();

            Assert.NotNull(scopedDependency);
        }

        [Fact]
        public async ValueTask DisposeCommandServicesDisposeDisposableDependencies()
        {
            var disposed = false;

            var disposable = new TestDisposableClass(() =>
            {
                disposed = true;
            });

            var provider = new ServiceCollection()
                                .AddSingleton(disposable)
                                .BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, scopeFactory);

            await commandsServices.DisposeAsync();

            Assert.True(disposed);
        }

        [Fact]
        public async ValueTask DisposeCommandServicesDisposeAsyncDisposableDependencies()
        {
            var disposed = false;

            var disposable = new TestAsyncDisposableClass(() =>
            {
                disposed = true;
            });

            var provider = new ServiceCollection()
                                .AddSingleton(disposable)
                                .BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, scopeFactory);

            await commandsServices.DisposeAsync();

            Assert.True(disposed);
        }
    }

    public class TestDisposableClass : IDisposable
    {
        private readonly Action _onDisposed;

        public TestDisposableClass(Action onDisposed)
        {
            _onDisposed = onDisposed;
        }

        public void Dispose()
        {
            _onDisposed();
        }
    }

    public class TestAsyncDisposableClass : IAsyncDisposable
    {
        private readonly Action _onDisposed;

        public TestAsyncDisposableClass(Action onDisposed)
        {
            _onDisposed = onDisposed;
        }

        public ValueTask DisposeAsync()
        {
            _onDisposed();

            return default;
        }
    }

    public class TestScopedDependency
    { }
}