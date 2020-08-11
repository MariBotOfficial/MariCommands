using System;
using System.Linq;
using MariCommands.TypeParsers;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    /// <summary>
    /// General extensions for dependency injection.
    /// </summary>
    public static class MariCommandsDependencyInjectionExtensions
    {
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
            return services.AddTransient<ITypeParser, TParser>();
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
            services.AddTransient<ITypeParser<T>, TParser>();
            services.AddTransient<ITypeParser>(sp =>
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
        public static IServiceCollection AddTypeParser<TParser, T>(this IServiceCollection services, bool createNullable)
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
        public static IServiceCollection TryAddTypeParser<TParser, T>(this IServiceCollection services, bool createNullable)
            where T : struct
            where TParser : class, ITypeParser<T>
        {
            if (services.Any(a => a.ServiceType == typeof(ITypeParser<T>)))
                return services;

            return services.AddTypeParser<TParser, T>(createNullable);
        }


        internal static T GetOrDefault<T, TDefault>(this IServiceProvider provider)
            where TDefault : T, new()
        {
            if (provider.HasNoContent())
                return new TDefault();

            return provider.GetService<T>() ?? new TDefault();
        }

        internal static T GetOrDefault<T>(this IServiceProvider provider, T defaultValue)
        {
            if (provider.HasNoContent())
                return defaultValue;

            return provider.GetService<T>() ?? defaultValue;
        }
    }
}