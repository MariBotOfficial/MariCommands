using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands
{
    public partial class CommandFactory : ICommandFactory
    {
        private CommandExecuteDelegate GetCommandDelegate(MethodInfo methodInfo)
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

        private CommandExecuteDelegate GetCommandValueTaskObjectDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                var result = await InvokeValueTaskObject(request.Instance, cmdCtx)
                                            .ConfigureAwait(false);

                // TODO: OkObjectResult
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandValueTaskResultDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                var result = await InvokeValueTaskResult(request.Instance, cmdCtx)
                                            .ConfigureAwait(false);

                request.Context.Result = result;
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandTaskObjectDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                var result = await InvokeTaskObject(request.Instance, cmdCtx)
                                            .ConfigureAwait(false);

                // TODO: OkObjectResult
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandTaskResultDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                var result = await InvokeTaskResult(request.Instance, cmdCtx)
                                            .ConfigureAwait(false);

                request.Context.Result = result;
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandValueTaskDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                await InvokeValueTask(request.Instance, cmdCtx)
                                        .ConfigureAwait(false);

                // TODO: Set OkResult.
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandTaskDelegate()
        {
            CommandExecuteDelegate commandDelegate = async request =>
            {
                var cmdCtx = request.Context.CommandContext;

                await InvokeTask(request.Instance, cmdCtx)
                                    .ConfigureAwait(false);

                // TODO: Set OkResult.
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandResultDelegate()
        {
            CommandExecuteDelegate commandDelegate = request =>
            {
                try
                {
                    var cmdCtx = request.Context.CommandContext;

                    var result = InvokeResult(request.Instance, cmdCtx);

                    request.Context.Result = result;

                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            };

            return commandDelegate;
        }

        private CommandExecuteDelegate GetCommandVoidDelegate()
        {
            CommandExecuteDelegate commandDelegate = request =>
            {
                try
                {
                    var cmdCtx = request.Context.CommandContext;

                    InvokeVoid(request.Instance, cmdCtx);
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

        private CommandExecuteDelegate GetCommandObjectDelegate()
        {
            CommandExecuteDelegate commandDelegate = request =>
            {
                try
                {
                    var cmdCtx = request.Context.CommandContext;

                    var result = InvokeObject(request.Instance, cmdCtx);

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

            await awaitable.ConfigureAwait(false);

            return (T)awaitable.GetAwaiter().GetResult();
        }

        private async ValueTask<T> InvokeGenericValueTask<T>(object instance, CommandContext context)
        {
            dynamic awaitable = context.Command.MethodInfo.Invoke(instance, context.Args.ToArray());

            await awaitable.ConfigureAwait(false);

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
            var moduleProvider = provider.GetOrDefault<IModuleProvider, ModuleProvider>();

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