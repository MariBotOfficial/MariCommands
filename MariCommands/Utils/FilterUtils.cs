using System;
using System.Threading.Tasks;
using MariCommands.Filters;

namespace MariCommands.Utils
{
    internal static class FilterUtils
    {
        public static async ValueTask SwitchDisposeAsync(object instance, Type factoryType)
        {
            if (factoryType == typeof(ServiceCommandFilterAttribute))
                return;

            switch (instance)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}