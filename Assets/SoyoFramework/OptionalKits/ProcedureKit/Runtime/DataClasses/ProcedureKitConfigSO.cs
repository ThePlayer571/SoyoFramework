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
            public List<int> TagIndices = new(); // 选中的Tag索引
            public List<int> AllowedPreviousIndices = new(); // AllowedPreviousProcedures索引
        }

        public List<string> TagNames = new();
        public List<ProcedureEntry> Procedures = new();

        private void OnEnable()
        {
            // 确保Entrance始终存在
            if (Procedures.Count == 0 || Procedures[0].Name != "Entrance")
            {
                Procedures.Insert(0, new ProcedureEntry { Name = "Entrance" });
            }
        }
    }
}