using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// The currently command of this context.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// The alias used for this command.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// The raw arguments of this context.
        /// </summary>
        public string RawArgs { get; set; }

        /// <summary>
        /// The parsed arguments.
        /// </summary>
        public IReadOnlyCollection<object> Args { get; set; }

        /// <summary>
        /// The dependency container of this context.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// The result of this execution context.
        /// </summary>
        public IResult Result { get; set; }

        /// <summary>
        /// A key/value collection to share data within the execution.
        /// </summary>
        public IDictionary<object, object> Items { get; set; }
    }
}