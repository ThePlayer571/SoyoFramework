using System.Collections.Generic;
using System.Linq;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor.Window
{
    public partial class ProcedureKitEditorWindow
    {
        private void LoadFromConfig()
        {
            // 加载 Tags
            _tags = new List<TagEntry>();
            foreach (var tag in _config.Tags)
            {
                _tags.Add(new TagEntry
                {
                    Name = tag.Name,
                    EnumValue = tag.EnumValue
                });
            }

            // 加载 Procedures
            _procedures = new List<ProcedureEntry>();
            foreach (var entry in _config.Procedures)
            {
                _procedures.Add(new ProcedureEntry
                {
                    Name = entry.Name,
                    EnumValue = entry.EnumValue,
                    TagEnumValues = new HashSet<int>(entry. TagEnumValues),
                    AllowedPreviousEnumValues = new List<int>(entry.AllowedPreviousEnumValues)
                });
            }

            // 确保 Entrance 存在
            if (_procedures.Count == 0 || _procedures[0].Name != "Entrance")
            {
                _procedures.Insert(0, new ProcedureEntry { Name = "Entrance", EnumValue = 0 });
            }

            // 确保 Entrance 的 EnumValue 为 0，且清除其 AllowedPreviousEnumValues
            _procedures[0].EnumValue = 0;
            _procedures[0].AllowedPreviousEnumValues.Clear();

            // 加载 Region 3 设置
            _generatePath = _config.GeneratePath;
            _namespace = _config.Namespace;
            _procedureIdEnumName = _config.ProcedureIdEnumName;
            _procedureTagEnumName = _config.ProcedureTagEnumName;
        }

        private void SaveToConfig()
        {
            // 保存 Tags
            _config.Tags = new List<ProcedureKitConfigSO.TagEntry>();
            foreach (var tag in _tags)
            {
                _config.Tags. Add(new ProcedureKitConfigSO.TagEntry
                {
                    Name = tag.Name,
                    EnumValue = tag.EnumValue
                });
            }

            // 保存 Procedures
            _config.Procedures = new List<ProcedureKitConfigSO.ProcedureEntry>();
            foreach (var proc in _procedures)
            {
                _config.Procedures.Add(new ProcedureKitConfigSO.ProcedureEntry
                {
                    Name = proc.Name,
                    EnumValue = proc.EnumValue,
                    TagEnumValues = proc.TagEnumValues. ToList(),
                    AllowedPreviousEnumValues = new List<int>(proc.AllowedPreviousEnumValues)
                });
            }

            // 保存 Region 3 设置
            _config.GeneratePath = _generatePath;
            _config. Namespace = _namespace;
            _config.ProcedureIdEnumName = _procedureIdEnumName;
            _config. ProcedureTagEnumName = _procedureTagEnumName;

            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssets();
            Debug.Log("配置已保存!");
        }
    }
}