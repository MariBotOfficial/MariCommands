using System;
using System.Linq;
using MariCommands.Builder;
using MariCommands.Factories;
using MariCommands.Filters;
using MariCommands.Parsers;
using MariCommands.Providers;
using MariCommands.TypeParsers;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MariCommands.Extensions
{
    /// <summary>
    /// General extensions for dependency injection.
    /// </summary>
    public static class MariCommandsDependencyInjectionExtensions
    {
        /// <summary>
        /// Add a filter factory to the dependency.
        /// </summary>
        /// <remarks>
        /// If <typeparamref name="TFilterFactory" /> is a generic filter factory (<see cref="IFilterFactory{TFilter, TFilterDelegate}" />)
        /// use <see cref="AddFilterFactory{TFilterFactory, TFilter, TFilterDelegate}(IServiceCollection)" /> instead this method.
        /// </remarks>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TFilterFactory">The filter factory to be injected.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddFilterFactory<TFilterFactory>(this IServiceCollection services)
            where TFilterFactory : class, IFilterFactory
        {
            return services.AddSingleton<IFilterFactory, TFilterFactory>();
        }

        /// <summary>
        /// Add a filter factory of the specified filter type to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TFilterFactory">The filter factory to be injected.</typeparam>
        /// <typeparam name="TFilter">The type of this filter factory can create delegates.</typeparam>
        /// <typeparam name="TFilterDelegate">The delegate type of the filter type.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddFilterFactory<TFilterFactory, TFilter, TFilterDelegate>(this IServiceCollection services)
            where TFilterFactory : class, IFilterFactory<TFilter, TFilterDelegate>
            where TFilter : ICommandFilter
            where TFilterDelegate : Delegate
        {
            services.AddSingleton<IFilterFactory<TFilter, TFilterDelegate>, TFilterFactory>();
            services.AddSingleton<IFilterFactory>(sp =>
            {
                return sp.GetRequiredService<IFilterFactory<TFilter, TFilterDelegate>>();
            });

            return services;
        }

        /// <summary>
        /// Add a filter factory of the specified filter type to the dependency if isn't already present
        /// a filter factory of the specified types.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TFilterFactory">The filter factory to be injected.</typeparam>
        /// <typeparam name="TFilter">The type of this filter factory can create delegates.</typeparam>
        /// <typeparam name="TFilterDelegate">The delegate type of the filter type.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection TryAddFilterFactory<TFilterFactory, TFilter, TFilterDelegate>(this IServiceCollection services)
            where TFilterFactory : class, IFilterFactory<TFilter, TFilterDelegate>
            where TFilter : ICommandFilter
            where TFilterDelegate : Delegate
        {
            if (services.Any(a => a.ServiceType == typeof(IFilterFactory<TFilter, TFilterDelegate>)))
                return services;

            return services.AddFilterFactory<TFilterFactory, TFilter, TFilterDelegate>();
        }

        /// <summary>
        /// Add a type parser to the dependency.
        /// </summary>
        /// <remarks>
        /// If <typeparamref name="TParser" /> is a generic type parser (<see cref="ITypeParser{T}" />)
        /// use <see cref="AddTypeParser{TParser, T}(IServiceCollection)" /> instead this method.
        /// </remarks>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TParser">The type parser to be injected.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddTypeParser<TParser>(this IServiceCollection services)
            where TParser : class, ITypeParser
        {
            return services.AddSingleton<ITypeParser, TParser>();
        }

        /// <summary>
        /// Add a type parser of the specified type to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TParser">The type parser to be injected.</typeparam>
        /// <typeparam name="T">The type of this type parser can parse.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddTypeParser<TParser, T>(this IServiceCollection services)
            where TParser : class, ITypeParser<T>
        {
            services.AddSingleton<ITypeParser<T>, TParser>();
            services.AddSingleton<ITypeParser>(sp =>
            {
                return sp.GetRequiredService<ITypeParser<T>>();
            });

            return services;
        }

        /// <summary>
        /// Add a type parser of the specified type to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <param name="createNullable">If this lib will create a nullable type parser for this type.</param>
        /// <typeparam name="TParser">The type parser to be injected.</typeparam>
        /// <typeparam name="T">The type of this type parser can parse.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddTypeParser<TParser, T>(this IServiceCollection services, bool createNullable = true)
            where T : struct
            where TParser : class, ITypeParser<T>
        {
            services.AddTypeParser<TParser, T>();

            if (createNullable)
                services.AddTypeParser<NullableTypeParser<T>, T?>();

            return services;
        }

        /// <summary>
        /// Add a type parser of the specified type to the dependency if isn't already present
        /// a type parser of the specified type.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <typeparam name="TParser">The type parser to be injected.</typeparam>
        /// <typeparam name="T">The type of this type parser can parse.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection TryAddTypeParser<TParser, T>(this IServiceCollection services)
            where TParser : class, ITypeParser<T>
        {
            if (services.Any(a => a.ServiceType == typeof(ITypeParser<T>)))
                return services;

            return services.AddTypeParser<TParser, T>();
        }

        /// <summary>
        /// Add a type parser of the specified type to the dependency if isn't already present
        /// a type parser of the specified type.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <param name="createNullable">If this lib will create a nullable type parser for this type.</param>
        /// <typeparam name="TParser">The type parser to be injected.</typeparam>
        /// <typeparam name="T">The type of this type parser can parse.</typeparam>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection TryAddTypeParser<TParser, T>(this IServiceCollection services, bool createNullable = true)
            where T : struct
            where TParser : class, ITypeParser<T>
        {
            if (services.Any(a => a.ServiceType == typeof(ITypeParser<T>)))
                return services;

            return services.AddTypeParser<TParser, T>(createNullable);
        }

        /// <summary>
        /// Adds all primitive type parsers to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <param name="createNullables">If this lib will create nullable type parsers for these 
        /// primitive type parsers.</param>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddPrimitiveTypeParsers(this IServiceCollection services, bool createNullables = true)
        {
            services.TryAddTypeParser<PrimitiveTypeParser<char>, char>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<bool>, bool>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<byte>, byte>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<sbyte>, sbyte>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<short>, short>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<ushort>, ushort>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<int>, int>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<uint>, uint>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<long>, long>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<ulong>, ulong>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<float>, float>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<double>, double>(createNullables);
            services.TryAddTypeParser<PrimitiveTypeParser<decimal>, decimal>(createNullables);

            return services;
        }

        /// <summary>
        /// Add the default string type parser to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddStringTypeParser(this IServiceCollection services)
            => services.TryAddTypeParser<StringTypeParser, string>();

        /// <summary>
        /// Add the default enum type parser to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddEnumTypeParser(this IServiceCollection services)
            => services.TryAddTypeParser<EnumTypeParser, Enum>();

        /// <summary>
        /// Add all default type parsers to the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <param name="createNullables">If this lib will create nullables type parsers for these type parsers.</param>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddAllDefaultTypeParsers(this IServiceCollection services, bool createNullables = true)
        {
            services.AddPrimitiveTypeParsers(createNullables);
            services.AddStringTypeParser();
            services.AddEnumTypeParser();

            return services;
        }

        /// <summary>
        /// Adds all basic services of the MariCommands for the dependency.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <param name="configureOptions">The configure options for this lib.</param>
        /// <returns>The current service collection.</returns>
        public static IServiceCollection AddBasicMariCommandsServices(this IServiceCollection services, Action<MariCommandsOptions> configureOptions = null)
        {
            services.AddLogging();
            services.AddOptions<MariCommandsOptions>();

            services.TryAddSingleton<IFilterProvider, FilterProvider>();
            services.TryAddSingleton<IContextExecutor, ContextExecutor>();
            services.TryAddSingleton<IModuleCache, ModuleCache>();
            services.TryAddSingleton(sp =>
            {
                return sp.GetRequiredService<IOptions<MariCommandsOptions>>().Value;
            });

            services.TryAddFilterFactory<ResultFilterFactory, ICommandResultFilter, CommandResultDelegate>();
            services.TryAddFilterFactory<ExceptionFilterFactory, ICommandExceptionFilter, CommandExceptionDelegate>();

            services.TryAddTransient<ICommandApplicationBuilderFactory, CommandApplicationBuilderFactory>();
            services.TryAddTransient<IModuleConfigurer, ModuleConfigurer>();
            services.TryAddTransient<IModuleFactory, ModuleFactory>();
            services.TryAddTransient<ICommandFactory, CommandFactory>();
            services.TryAddTransient<IParameterFactory, ParameterFactory>();
            services.TryAddTransient<IArgumentParser, ArgumentParser>();
            services.TryAddTransient<ITypeParserProvider, TypeParserProvider>();
            services.TryAddTransient<ICommandExecutorProvider, CommandExecutorProvider>();

            services.TryAddScoped<ICommandContextAccessor, ScopedCommandContextAccessor>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(DisposeFilter), int.MinValue);
            });

            var builder = new MariCommandsOptions();

            if (configureOptions.HasContent())
            {
                configureOptions(builder);

                services.Configure(configureOptions);
            }

            if (builder.AddAllDefaultTypeParsers)
                services.AddAllDefaultTypeParsers(builder.CreateNullables);

            return services;
        }

        /// <summary>
        /// Initialize the MariCommands lib with the specified service provider.
        /// Obs: Don't use this method for host builders.
        /// </summary>
        /// <remarks>
        /// Don't use this method for host builders
        /// </remarks>
        /// <param name="provider">The current container service provider.</param>
        public static void InitializeCommandApp(this IServiceProvider provider)
            => ApplicationBuilderUtils.InitializeCommandApp(provider);
    }
}