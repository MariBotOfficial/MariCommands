using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;

namespace MariCommands.Builder
{
    /// <inheritdoc />
    public class CommandApplicationBuilder : ICommandApplicationBuilder
    {
        private readonly List<Func<CommandDelegate, CommandDelegate>> _components;

        /// <summary>
        /// Creates a new instance of <see cref="CommandApplicationBuilder" />.
        /// </summary>
        /// <param name="properties">The properties of this builder.</param>
        /// <param name="applicationServices">The service container for this application.</param>
        public CommandApplicationBuilder(IDictionary<string, object> properties, IServiceProvider applicationServices)
        {
            Properties = properties;
            ApplicationServices = applicationServices;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CommandApplicationBuilder" />.
        /// </summary>
        /// <param name="applicationServices">The service container for this application.</param>
        public CommandApplicationBuilder(IServiceProvider applicationServices)
            : this(new Dictionary<string, object>(), applicationServices)
        {
        }

        /// <inheritdoc />
        public IDictionary<string, object> Properties { get; }

        /// <inheritdoc />
        public IServiceProvider ApplicationServices { get; }

        /// <inheritdoc />
        public ICommandApplicationBuilder Use(Func<CommandDelegate, CommandDelegate> component)
        {
            _components.Add(component);
            return this;
        }

        /// <inheritdoc />
        public CommandDelegate Build()
        {
            CommandDelegate app = context =>
            {
                context.NotNull(nameof(context));

                var command = context.Command;

                if (command.HasNoContent())
                {
                    // TODO: Put the parser middleware here.
                    var message =
                        $"The request reached the end of the pipeline without a command defined";

                    throw new InvalidOperationException(message);
                }

                if (context.Result.HasNoContent())
                {
                    // TODO: Put the executor middleware here.
                    var message =
                        $"The request reached the end of the pipeline without executing the command: '{command.Name}'.";

                    throw new InvalidOperationException(message);
                }

                return Task.CompletedTask;
            };

            _components.Reverse();

            foreach (var component in _components)
                app = component(app);

            return app;
        }
    }
}