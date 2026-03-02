#if XNODE_SUPPORT

using UnityEngine;
using XNode;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Nodes
{
    public class ProcedureNode : Node
    {
        [SerializeField] [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private int previous;

        [SerializeField] [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
        private int next;

        [SerializeField, HideInInspector] private int _enumValue;

        public override object GetValue(NodePort port)
        {
            return _enumValue;
        }

        public int EnumValue
        {
            get => _enumValue;
            set => _enumValue = value;
        }
    }
}

#endif