#if UNITY_EDITOR


using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.UITest
{
    public interface IUITestLogic
    {
        UniTask ExecuteAsync();
    }

    public abstract class UITestLogicAsset : ScriptableObject, IUITestLogic
    {
        public abstract UniTask ExecuteAsync();
    }
}
#endif