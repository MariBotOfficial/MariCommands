using MariCommands.Results;
using Microsoft.Extensions.Logging;

namespace MariCommands.Utils
{
    internal static class MiddlewareUtils
    {
        public static bool VerifyMatchDisabled(CommandContext context, ICommandMatch match, ILogger logger = null)
        {
            if (!match.Command.IsEnabled)
            {
                logger.LogInformation("The matched command is disabled.");
                context.Result = CommandDisabledResult.FromCommand(match.Command);

                return true;
            }

            return true;
        }
    }
}