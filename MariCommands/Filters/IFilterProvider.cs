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
        /// <typeparam name="TContext">The filter context type.</typeparam>
        /// <typeparam name="TFilter">The type of filter to be invoked.</typeparam>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        Task InvokeFiltersAsync<TContext, TFilter>(TContext context)
            where TFilter : ICommandFilter
            where TContext : IFilterContext;
    }
}