using System.Linq;
using SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Nodes;
using UnityEngine;
using XNodeEditor;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Editor
{
    [CustomNodeEditor(typeof(ProcedureNode))]
    public class ProcedureNodeEditor : NodeEditor
    {
        public override void OnHeaderGUI()
        {
            var node = target as ProcedureNode;
            var graph = node.graph as ProcedureChangeRuleGraph;

            string procedureName;
            if (graph.AttachedConfig == null)
            {
                procedureName = "Null";
            }
            else
            {
                procedureName =
                    graph.AttachedConfig.Procedures.FirstOrDefault(x => x.EnumValue == node.EnumValue)?.Name ?? "Null";
            }

            GUILayout.Label($"{node.EnumValue} {procedureName}", NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
    }
}