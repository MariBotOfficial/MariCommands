using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    internal sealed class ResultFilterFactory : BaseFilterFactory<ICommandResultFilter, CommandResultDelegate>
    {
        private readonly SemaphoreSlim _delegateLock;

        public ResultFilterFactory(IOptions<MariCommandsOptions> options) : base(options)
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
                                                CommandResultDelegate component(CommandResultDelegate next)
                                                {
                                                    var factory = a;
                                                    var factoryType = factory.GetType();

                                                    return async ctx =>
                                                    {
                                                        var instance = (ICommandResultFilter)factory.CreateInstance(ctx.CommandContext.CommandServices);

                                                        await instance.InvokeAsync(ctx, next);

                                                        await FilterUtils.SwitchDisposeAsync(instance, factoryType);
                                                    };
                                                }

                                                return (Func<CommandResultDelegate, CommandResultDelegate>)component;
                                            })
                                            .ToList();

            CommandResultDelegate commandResultDelegate = ctx =>
            {
                return Task.CompletedTask;
            };

            foreach (var component in components)
                commandResultDelegate = component(commandResultDelegate);

            Delegate = commandResultDelegate;

            _delegateLock.Release();
        }
    }
}