using SoyoFramework.Framework.Runtime.Core.CommandProfiler;
using UnityEditor;

namespace SoyoFramework.Framework.Editor.CommandProfiler
{
    [InitializeOnLoad]
    internal static class CommandAnalyzerPlayModeHandler
    {
        static CommandAnalyzerPlayModeHandler()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (CommandAnalyzerSettings.instance.AutoEnableOnPlay)
                {
                    CommandSendHook.Enabled = true;
                }
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // 退出 Play 模式时关闭记录
                CommandSendHook.Enabled = false;
            }
        }
    }
}