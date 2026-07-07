using UnityEngine;

namespace SoyoFramework.ToolKits
{
    public interface IGameObject
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}