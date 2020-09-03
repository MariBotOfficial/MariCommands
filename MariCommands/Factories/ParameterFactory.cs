using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MariCommands.Extensions;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Factories
{
    /// <inheritdoc />
    internal sealed class ParameterFactory : IParameterFactory
    {
        private readonly MariCommandsOptions _config;

        public ParameterFactory(IOptions<MariCommandsOptions> config)
        {
            _config = config.Value;
        }

        /// <inheritdoc />
        public IParameterBuilder BuildParameter(ICommandBuilder command, ParameterInfo parameterInfo)
        {
            command.NotNull(nameof(command));
            parameterInfo.NotNull(nameof(parameterInfo));

            if (!IsParameter(command, parameterInfo))
                throw new ArgumentException(nameof(parameterInfo), $"{parameterInfo.Name} is not a valid parameter.");

            var name = GetName(parameterInfo);
            var description = GetDescription(parameterInfo);
            var remarks = GetRemarks(parameterInfo);
            var isRemainder = GetRemainder(parameterInfo);
            var isParams = GetParams(parameterInfo);
            var isOptional = GetOptional(parameterInfo);
            var defaultValue = GetDefaultValue(parameterInfo);
            var typeParserType = GetTypeParserType(parameterInfo);
            var attributes = GetAttributes(parameterInfo);
            var preconditions = GetPreconditions(attributes);

            var builder = new ParameterBuilder()
                                .WithParameterInfo(parameterInfo)
                                .WithName(name)
                                .WithDescription(description)
                                .WithRemarks(remarks)
                                .WithRemainder(isRemainder)
                                .WithParams(isParams)
                                .WithOptional(isOptional)
                                .WithDefaultValue(defaultValue)
                                .WithTypeParserType(typeParserType)
                                .WithAttributes(attributes)
                                .WithPreconditions(preconditions);

            return builder;
        }

        private IEnumerable<ParamPreconditionAttribute> GetPreconditions(IEnumerable<Attribute> attributes)
        {
            return attributes
                    .Where(a => typeof(ParamPreconditionAttribute).IsAssignableFrom(a.GetType()))
                    .Select(a => a as ParamPreconditionAttribute)
                    .ToList();
        }

        private IEnumerable<Attribute> GetAttributes(ParameterInfo parameterInfo)
        {
            return parameterInfo.GetCustomAttributes();
        }

        private Type GetTypeParserType(ParameterInfo parameterInfo)
        {
            var typeParserAttr = parameterInfo.GetAttribute<TypeParserAttribute>();

            // Can be null, if null we just will use the default or the injected in dependency.
            return typeParserAttr?.Value;
        }

        private object GetDefaultValue(ParameterInfo parameterInfo)
        {
            if (parameterInfo.HasDefaultValue)
                return parameterInfo.DefaultValue;

            return null;
        }

        private bool GetOptional(ParameterInfo parameterInfo)
        {
            return parameterInfo.HasDefaultValue;
        }

        private bool GetParams(ParameterInfo parameterInfo)
        {
            var paramsAttr = parameterInfo.GetAttribute<ParamArrayAttribute>();

            if (paramsAttr.HasContent())
                return true;

            return false;
        }

        private bool GetRemainder(ParameterInfo parameterInfo)
        {
            var remainderAttr = parameterInfo.GetAttribute<RemainderAttribute>();

            if (remainderAttr.HasContent())
                return true;

            return false;
        }

        private string GetRemarks(ParameterInfo parameterInfo)
        {
            var remarksAttr = parameterInfo.GetAttribute<RemarksAttribute>();

            if (remarksAttr.HasContent())
                return remarksAttr.Value;

            return string.Empty;
        }

        private string GetDescription(ParameterInfo parameterInfo)
        {
            var descriptionAttr = parameterInfo.GetAttribute<DescriptionAttribute>();

            if (descriptionAttr.HasContent())
                return descriptionAttr.Value;

            return string.Empty;
        }

        private string GetName(ParameterInfo parameterInfo)
        {
            var nameAttr = parameterInfo.GetAttribute<NameAttribute>();

            if (nameAttr.HasContent())
                return nameAttr.Value;

            return parameterInfo.Name;
        }

        /// <inheritdoc />
        public bool IsParameter(ICommandBuilder command, ParameterInfo parameterInfo)
        {
            var isValid =
                        command.HasContent() &&
                        parameterInfo.HasContent();

            return isValid;
        }
    }
}