using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// Represents a module that can have commands.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// The name for this module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description for this module.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Any remarks for this module.
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// The <see cref="RunMode" /> for this module.
        /// </summary>
        RunMode RunMode { get; }

        /// <summary>
        /// If this module will ignore extra args.
        /// </summary>
        bool IgnoreExtraArgs { get; }

        /// <summary>
        /// The lifetime for this module.
        /// </summary>
        ModuleLifetime ModuleLifetime { get; }

        /// <summary>
        /// The default argument parser type for this module.
        /// </summary>
        Type ArgumentParserType { get; }

        /// <summary>
        /// Defines how multi matches will be handled in this module.
        /// </summary>
        MultiMatchHandling MultiMatchHandling { get; }

        /// <summary>
        /// The separator for this module.
        /// </summary>
        string Separator { get; }

        /// <summary>
        /// The default comparison for search commands in this module.
        /// </summary>
        StringComparison? Comparison { get; }

        /// <summary>
        /// Aliases for this module.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Submoduels for this module.
        /// </summary>
        IReadOnlyCollection<IModule> Submodules { get; }

        /// <summary>
        /// All atributtes of this module.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// All comands of this module.
        /// </summary>
        IReadOnlyCollection<ICommand> Commands { get; }

        /// <summary>
        /// All preconditions to be passed for this module.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// The parent module of this module.
        /// </summary>
        IModule Parent { get; }

        /// <summary>
        /// The real <see cref="Type" /> of this module.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Indicates if this module is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Asynchronously enable this module.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task EnableAsync();

        /// <summary>
        /// Asynchronously disable this module.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task DisableAsync();
    }
}