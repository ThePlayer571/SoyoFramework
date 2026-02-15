using Cysharp.Threading.Tasks;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEngine;

namespace Examples.UIKitExample.Scripts
{
    public class UIKitExampleInitializer : MonoBehaviour
    {
        [SerializeField] private UISettings uiSettings;

        private void Awake()
        {
            UIKit.Init(uiSettings);
            UIKit.OpenPageAsync("MainPage").Forget();
        }
    }
}