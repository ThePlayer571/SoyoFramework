using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Utils
{
    public interface IGameObject
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}