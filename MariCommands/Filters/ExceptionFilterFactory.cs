using System;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    internal sealed class ExceptionFilterFactory : BaseFilterFactory<ICommandExceptionFilter, CommandExceptionDelegate>
    {
        private CommandExceptionDelegate _delegate;

        public ExceptionFilterFactory(IOptions<MariCommandsOptions> options) : base(options)
        {
        }

        public override CommandExceptionDelegate GetFiltersDelegate()
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
                                                CommandExceptionDelegate component(CommandExceptionDelegate next)
                                                {
                                                    return async ctx =>
                                                    {
                                                        var instance = (ICommandExceptionFilter)a.CreateInstance(ctx.CommandContext.CommandServices);

                                                        await instance.InvokeAsync(ctx, next);

                                                        await MiddlewareUtils.SwitchDisposeAsync(instance);
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

            _delegate = commandExceptionDelegate;
        }
    }
}