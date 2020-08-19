using System;

namespace MariCommands.Features
{
    internal sealed class ExceptionHandlerFeature : IExceptionHandlerFeature
    {
        public ExceptionHandlerFeature(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; }
    }
}