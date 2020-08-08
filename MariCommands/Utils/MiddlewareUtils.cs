using Microsoft.Extensions.Logging;

namespace MariCommands.Utils
{
    internal static class MiddlewareUtils
    {
        public const string COMMAND_MATCH = "CommandMatch";
        public const string COMMAND_MATCHES = "CommandMatches";

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