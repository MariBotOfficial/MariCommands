using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    internal sealed class ResultFilterFactory : BaseFilterFactory<ICommandResultFilter, CommandResultDelegate>
    {
        private CommandResultDelegate _delegate;

        public ResultFilterFactory(IOptions<MariCommandsOptions> options) : base(options)
        {
        }

        public override CommandResultDelegate GetFiltersDelegate()
        {
            if (_delegate.HasContent())
                return _delegate;

            BuildDelegate();

            return _delegate;
        }

        private void BuildDelegate()
        {
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

            _delegate = commandResultDelegate;
        }
    }
}