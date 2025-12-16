using UnityEditor;

namespace SoyoFramework.Framework.Editor.CommandProfiler
{
    [FilePath("SoyoFramework/CommandAnalyzerSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class CommandAnalyzerSettings : ScriptableSingleton<CommandAnalyzerSettings>
    {
        /// <summary>
        /// 进入 Play 模式后是否自动开启记录
        /// </summary>
        public bool AutoEnableOnPlay = false;

        public void Save()
        {
            Save(true);
        }
    }
}