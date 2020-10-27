using System;
using System.Runtime.ExceptionServices;

namespace MariCommands.Filters
{
    public class CommandExceptionContext
    {
        public CommandExceptionContext(CommandContext commandContext, ExceptionDispatchInfo exceptionDispatchInfo)
        {
            ExceptionDispatchInfo = exceptionDispatchInfo;
            CommandContext = commandContext;
        }

        public virtual ExceptionDispatchInfo ExceptionDispatchInfo { get; set; }

        public virtual Exception Exception { get; set; }

        public virtual CommandContext CommandContext { get; }

        public virtual bool ExceptionHandled { get; set; }
    }
}