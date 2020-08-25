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
        public async Task DisposablesWillDisposeIDisposable()
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
        public async Task DisposablesWillDisposeIAsyncDisposable()
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
        public async Task DisposeDisposablesWillClearRegisteredDisposables()
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
        public async Task DisposeDisposablesWillClearRegisteredAsyncDisposables()
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
        public async Task DisposeCommandServicesDisposeDisposableDependencies()
        {
            var disposed = false;



            var provider = new ServiceCollection()
                                .AddScoped<TestDisposableClass>((sp) =>
                                {
                                    return new TestDisposableClass(() =>
                                    {
                                        disposed = true;
                                    });
                                })
                                .BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, scopeFactory);

            var disposable = commandsServices.CommandServices.GetRequiredService<TestDisposableClass>();

            await commandsServices.DisposeAsync();

            Assert.True(disposed);
        }

        [Fact]
        public async Task DisposeCommandServicesDisposeAsyncDisposableDependencies()
        {
            var disposed = false;

            var provider = new ServiceCollection()
                                .AddScoped<TestAsyncDisposableClass>((sp) =>
                                {
                                    return new TestAsyncDisposableClass(() =>
                                    {
                                        disposed = true;
                                    });
                                })
                                .BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext();
            var commandsServices = new CommandServicesFeature(context, scopeFactory);

            var asyncDisposable = commandsServices.CommandServices.GetRequiredService<TestAsyncDisposableClass>();

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