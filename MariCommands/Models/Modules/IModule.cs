using System;
using System.Collections.Generic;
using MariGlobals.Extensions;

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
        RunMode? RunMode { get; }

        /// <summary>
        /// If this module will ignore extra args.
        /// </summary>
        bool? IgnoreExtraArgs { get; }

        /// <summary>
        /// The default argument parser type for this module.
        /// </summary>
        Type ArgumentParserType { get; }

        /// <summary>
        /// Defines how multi matches will be handled in this module.
        /// </summary>
        MultiMatchHandling? MultiMatchHandling { get; }

        /// <summary>
        /// Aliases for this module.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Submodules for this module.
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
        /// Enable this module.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable this module.
        /// </summary>
        void Disable();

        /// <summary>
        /// Gets the <see cref="RunMode" /> for this module.
        /// </summary>
        /// <param name="options">The options with default <see cref="RunMode" /> value.</param>
        /// <returns>The <see cref="RunMode" /> for this module.</returns>
        RunMode GetRunMode(MariCommandsOptions options)
        {
            if (RunMode.HasValue)
                return RunMode.Value;

            if (Parent.HasContent())
                return Parent.GetRunMode(options);

            return options?.RunMode ?? new MariCommandsOptions().RunMode;
        }

        /// <summary>
        /// Gets ifs this module ignore extra args.
        /// </summary>
        /// <param name="options">The options with default ignore extra args value.</param>
        /// <returns><see langword="true" /> if this module ignore extra args.</returns>
        bool GetIgnoreExtraArgs(MariCommandsOptions options)
        {
            if (IgnoreExtraArgs.HasValue)
                return IgnoreExtraArgs.Value;

            if (Parent.HasContent())
                return Parent.GetIgnoreExtraArgs(options);

            return options?.IgnoreExtraArgs ?? new MariCommandsOptions().IgnoreExtraArgs;
        }

        /// <summary>
        /// Gets the <see cref="MultiMatchHandling" /> for this module.
        /// </summary>
        /// <param name="options">The options with default <see cref="MultiMatchHandling" /> value.</param>
        /// <returns>The <see cref="MultiMatchHandling" /> for this module.</returns>
        MultiMatchHandling GetMatchHandling(MariCommandsOptions options)
        {
            if (MultiMatchHandling.HasValue)
                return MultiMatchHandling.Value;

            if (Parent.HasContent())
                return Parent.GetMatchHandling(options);

            return options?.MatchHandling ?? new MariCommandsOptions().MatchHandling;
        }

        /// <summary>
        /// Gets the default argument parser type for this module.
        /// </summary>
        /// <returns>The default argument parser type for this module (can be <see langword="null" />).</returns>
        Type GetArgumentParserType()
        {
            if (ArgumentParserType.HasContent())
                return ArgumentParserType;
            else if (Parent.HasContent())
                return Parent.GetArgumentParserType();
            else
                return null;
        }

        /// <summary>
        /// Gets all preconditions for this module.
        /// </summary>
        /// <returns>All preconditions for this module.</returns>
        IReadOnlyCollection<PreconditionAttribute> GetAllPreconditions()
        {
            var preconditions = new List<PreconditionAttribute>();

            preconditions.AddRange(Preconditions);

            if (Parent.HasContent())
                preconditions.AddRange(Parent.GetAllPreconditions());

            return preconditions;
        }
    }
}