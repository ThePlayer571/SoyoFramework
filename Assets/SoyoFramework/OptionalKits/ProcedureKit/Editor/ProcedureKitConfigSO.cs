using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses
{
    [CreateAssetMenu(fileName = "ProcedureKitConfig", menuName = "SoyoFramework/ProcedureKit/Config")]
    public class ProcedureKitConfigSO : ScriptableObject
    {
        [Serializable]
        public class ProcedureEntry
        {
            public string Name;
            public int EnumValue; // 用户指定的 enum int 值
            public List<int> TagEnumValues = new(); // 选中的Tag的EnumValue（不再是index）
            public List<int> AllowedPreviousEnumValues = new(); // AllowedPreviousProcedures的EnumValue
        }

        [Serializable]
        public class TagEntry
        {
            public string Name;
            public int EnumValue; // 用户指定的 enum int 值
        }

        [Header("Procedure Data")] public List<TagEntry> Tags = new();
        public List<ProcedureEntry> Procedures = new();

        [Header("Code Generation Settings")] public string GeneratePath = "Assets/Scripts/ProcedureKit/Generated";
        public string Namespace = "Game.Procedures";
        public string ProcedureIdEnumName = "ProcedureId";
        public string ProcedureTagEnumName = "ProcedureTag";

        private void OnEnable()
        {
            // 确保Entrance始终存在
            if (Procedures.Count == 0 || Procedures[0].Name != "Entrance")
            {
                Procedures.Insert(0, new ProcedureEntry { Name = "Entrance", EnumValue = 0 });
            }
        }
    }
}