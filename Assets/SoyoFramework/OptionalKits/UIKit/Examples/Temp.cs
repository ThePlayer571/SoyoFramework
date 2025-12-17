using Cysharp.Threading.Tasks;
using SoyoFramework.OptionalKits.UIKit.Runtime;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Examples
{
    public class Temp : MonoBehaviour
    {
        public UISettings UISettings;

        private void Start()
        {
            Runtime.UIKit.Init(UISettings);
            Runtime.UIKit.OpenPageAsync<TestPage>("TestPage").Forget();
        }
    }
}