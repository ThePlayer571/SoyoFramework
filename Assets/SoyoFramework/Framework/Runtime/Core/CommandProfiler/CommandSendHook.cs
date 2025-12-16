namespace SoyoFramework.Framework.Runtime.Core.CommandProfiler
{
    /// <summary>
    /// SendCommand 的唯一统计入口
    /// </summary>
    internal static class CommandSendHook
    {
        internal static bool Enabled;

        internal static void OnSend(ICommand command)
        {
            if (!Enabled || command == null)
                return;

            CommandStatsStore.Record(command.GetType());
        }

        internal static void OnSend<TResult>(ICommand<TResult> command)
        {
            if (!Enabled || command == null)
                return;

            CommandStatsStore.Record(command.GetType());
        }
    }
}