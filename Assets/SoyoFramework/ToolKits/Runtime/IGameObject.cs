using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime
{
    public interface IGameObject
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}