#if XNODE_SUPPORT

using UnityEngine;
using XNode;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Nodes
{
    [NodeTint("#599F41")]
    public class EntranceNode : Node
    {
        [SerializeField] [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private int next;
    }
}

#endif