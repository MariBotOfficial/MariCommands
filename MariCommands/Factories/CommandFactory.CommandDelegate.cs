using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    public partial class CommandFactory : ICommandFactory
    {
        private CommandDelegate GetCommandDelegate(MethodInfo methodInfo)
        {
            var returnType = methodInfo.ReturnType;

            if (returnType.HasNoContent() || returnType == typeof(void))
                return GetCommandVoidDelegate();

            if (typeof(IResult).IsAssignableFrom(returnType))
                return GetCommandResultDelegate();

            if (returnType == typeof(Task) && !returnType.IsGenericType)
                return GetCommandTaskDelegate();

            if (returnType == typeof(ValueTask) && !returnType.IsGenericType)
                return GetCommandValueTaskDelegate();

            if (returnType.IsGenericType)
            {
                var genericDefinition = returnType.GetGenericTypeDefinition();

                var genericTypes = returnType.GetGenericArguments();

                var isNotTaskOrValueTask =
                        genericDefinition != typeof(Task<>) &&
                        genericDefinition != typeof(ValueTask<>);

                if (genericTypes.Count() > 1 || !isNotTaskOrValueTask)
                    return GetCommandObjectDelegate();

                var genericType = genericTypes.FirstOrDefault();
                var isTask = genericDefinition == typeof(Task<>);


                if (typeof(IResult).IsAssignableFrom(genericType) && isTask)
                    return GetCommandTaskResultDelegate();
                if (isTask)
                    return GetCommandTaskObjectDelegate();
                if (typeof(IResult).IsAssignableFrom(genericType))
                    return GetCommandValueTaskResultDelegate();

                return GetCommandValueTaskObjectDelegate();
            }

            return GetCommandObjectDelegate();
        }

        private CommandDelegate GetCommandValueTaskObjectDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                var result = await InvokeValueTaskObject(instance, context.CommandContext);

                // TODO: OkObjectResult
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandValueTaskResultDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                var result = await InvokeValueTaskResult(instance, context.CommandContext);

                context.Result = result;
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandTaskObjectDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                var result = await InvokeTaskObject(instance, context.CommandContext);

                // TODO: OkObjectResult
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandTaskResultDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                var result = await InvokeTaskResult(instance, context.CommandContext);

                context.Result = result;
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandValueTaskDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                await InvokeValueTask(instance, context.CommandContext);

                // TODO: Set OkResult.
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandTaskDelegate()
        {
            CommandDelegate commandDelegate = async context =>
            {
                var instance = PrepareExecution(context);

                await InvokeTask(instance, context.CommandContext);

                // TODO: Set OkResult.
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandResultDelegate()
        {
            CommandDelegate commandDelegate = context =>
            {
                try
                {
                    var instance = PrepareExecution(context);
                    var result = InvokeResult(instance, context.CommandContext);

                    context.Result = result;

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandVoidDelegate()
        {
            CommandDelegate commandDelegate = context =>
            {
                try
                {
                    var instance = PrepareExecution(context);

                    InvokeVoid(instance, context.CommandContext);
                    // TODO: Set OkResult.

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            };

            return commandDelegate;
        }

        private CommandDelegate GetCommandObjectDelegate()
        {
            CommandDelegate commandDelegate = context =>
            {
                try
                {
                    var instance = PrepareExecution(context);

                    var result = InvokeObject(instance, context.CommandContext);

                    // TODO: Set OkObjectResult.

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            };

            return commandDelegate;
        }

        private ValueTask<object> InvokeValueTaskObject(object instance, CommandContext context)
            => InvokeGenericValueTask<object>(instance, context);

        private ValueTask<IResult> InvokeValueTaskResult(object instance, CommandContext context)
            => InvokeGenericValueTask<IResult>(instance, context);

        private Task<object> InvokeTaskObject(object instance, CommandContext context)
            => InvokeGenericTask<object>(instance, context);

        private Task<IResult> InvokeTaskResult(object instance, CommandContext context)
            => InvokeGenericTask<IResult>(instance, context);

        private async Task<T> InvokeGenericTask<T>(object instance, CommandContext context)
        {
            dynamic awaitable = context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

            await awaitable;

            return (T)awaitable.GetAwaiter().GetResult();
        }

        private async ValueTask<T> InvokeGenericValueTask<T>(object instance, CommandContext context)
        {
            dynamic awaitable = context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

            await awaitable;

            return (T)awaitable.GetAwaiter().GetResult();
        }

        private ValueTask InvokeValueTask(object instance, CommandContext context)
            => (ValueTask)context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

        private Task InvokeTask(object instance, CommandContext context)
            => (Task)context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

        private IResult InvokeResult(object instance, CommandContext context)
            => (IResult)context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

        private object InvokeObject(object instance, CommandContext context)
            => context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

        private void InvokeVoid(object instance, CommandContext context)
            => context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

        private object PrepareExecution(CommandExecutionContext context)
        {
            var cmdCtx = context.CommandContext;
            var provider = cmdCtx.ServiceProvider;
            var moduleProvider = provider.GetOrDefault<IModuleProvider>(new ModuleProvider(provider));

            var instance = moduleProvider.Instantiate(context);

            if (!instance.OfType<IModuleBase>())
            {
                var message =
                    $"Cannot instantiate {cmdCtx.Command.Module.Name} because this module doesn't" +
                    $"implements {nameof(IModuleBase)}, see if your module inherits ModuleBase<T>.";

                throw new InvalidCastException(message);
            }

            (instance as IModuleBase).SetContext(cmdCtx);

            return instance;
        }
    }
}