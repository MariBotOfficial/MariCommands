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
                                            .OrderBy(a => a is IOrderedCommandFilter orderedCommandFilter
                                                    ? orderedCommandFilter.Order
                                                    : 0
                                            )
                                            .Select(a =>
                                            {
                                                CommandResultDelegate component(CommandResultDelegate next)
                                                {
                                                    return async ctx =>
                                                    {
                                                        var instance = (ICommandResultFilter)a.CreateInstance(ctx.CommandContext.CommandServices);

                                                        await instance.InvokeAsync(ctx, next);

                                                        await MiddlewareUtils.SwitchDisposeAsync(instance);
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