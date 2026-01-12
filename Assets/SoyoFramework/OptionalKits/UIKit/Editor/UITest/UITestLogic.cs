using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Editor.UITest
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