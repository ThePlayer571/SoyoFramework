#if XNODE_SUPPORT

using SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Nodes;
using UnityEngine;
using XNodeEditor;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Editor
{
    [CustomNodeEditor(typeof(EntranceNode))]
    public class EntranceNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUILayout.Label("Entrance", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
    }
}

#endif