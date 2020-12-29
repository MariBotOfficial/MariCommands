using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    internal sealed class ExceptionFilterFactory : BaseFilterFactory<ICommandExceptionFilter, CommandExceptionDelegate>
    {
        private readonly SemaphoreSlim _delegateLock;

        public ExceptionFilterFactory(IOptions<MariCommandsOptions> options) : base(options)
        {
            _delegateLock = new SemaphoreSlim(1, 1);
        }

        protected override void BuildDelegate()
        {
            if (_delegateLock.CurrentCount <= 0)
            {
                // if we alreading building the delegate wait until the delegate is ready.
                _delegateLock.Wait();
                _delegateLock.Release();
                return;
            }

            _delegateLock.Wait();

            var filterFactories = GetFilterFactories();

            var components = filterFactories
                                            .Select(a =>
                                            {
                                                CommandExceptionDelegate component(CommandExceptionDelegate next)
                                                {
                                                    var factory = a;
                                                    var factoryType = factory.GetType();

                                                    return async ctx =>
                                                    {
                                                        var instance = (ICommandExceptionFilter)factory.CreateInstance(ctx.CommandContext.CommandServices);

                                                        await instance.InvokeAsync(ctx, next);

                                                        await FilterUtils.SwitchDisposeAsync(instance, factoryType);
                                                    };
                                                }

                                                return (Func<CommandExceptionDelegate, CommandExceptionDelegate>)component;
                                            })
                                            .ToList();

            CommandExceptionDelegate commandExceptionDelegate = ctx =>
            {
                return Task.CompletedTask;
            };

            foreach (var component in components)
                commandExceptionDelegate = component(commandExceptionDelegate);

            Delegate = commandExceptionDelegate;

            _delegateLock.Release();
        }
    }
}