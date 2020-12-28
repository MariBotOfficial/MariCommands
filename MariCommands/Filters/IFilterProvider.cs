using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// A provider that can execute all filters to the specified context.
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Invoke all filters for the specified context.
        /// </summary>
        /// <param name="context">The context to be filtered.</param>
        /// <param name="filterType">The type of filter to be invoked.</param>
        /// <typeparam name="TContext">The filter context type.</typeparam>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task InvokeFiltersAsync<TContext>(TContext context, CommandFilterType filterType)
            where TContext : IFilterContext;
    }
}