using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Executors;

namespace MariCommands
{
    /// <summary>
    /// Represents a pre builded command.
    /// </summary>
    public interface ICommandBuilder
    {
        /// <summary>
        /// The name for this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description for this command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Any remarks for this command.
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// The priority of this command.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// The <see cref="RunMode" /> for this command.
        /// </summary>
        RunMode? RunMode { get; }

        /// <summary>
        /// If this command will ignore extra args.
        /// </summary>
        bool? IgnoreExtraArgs { get; }

        /// <summary>
        /// The default argument parser type for this command.
        /// </summary>
        Type ArgumentParserType { get; }

        /// <summary>
        /// Aliases for this command.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// All atributtes of this command.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// All preconditions to be passed for this command.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// All parameters of this command.
        /// </summary>
        IReadOnlyCollection<IParameterBuilder> Parameters { get; }

        /// <summary>
        /// Indicates if this command is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Indicates if this command is async 
        /// (is a <see cref="Task" /> or a <see cref="ValueTask" />).
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// The generic return type of this command.
        /// </summary>
        /// <remarks>
        /// May be null if <see cref="IsAsync"/> is <see langword="false"/>.
        /// </remarks>
        Type AsyncResultType { get; }

        /// <summary>
        /// The executor for this command.
        /// </summary>
        ICommandExecutor Executor { get; }

        /// <summary>
        /// The real <see cref="MethodInfo" /> of this command.
        /// </summary>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// The module of this command.
        /// </summary>
        IModuleBuilder Module { get; }

        /// <summary>
        /// Finishes the building of this command.
        /// </summary>
        ICommand Build(IModule module);
    }
}