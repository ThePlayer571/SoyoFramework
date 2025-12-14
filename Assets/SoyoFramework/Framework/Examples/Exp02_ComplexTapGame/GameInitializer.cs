using UnityEngine;

namespace SoyoFramework.Framework.Examples.Exp02_ComplexTapGame
{
    public class GameInitializer : MonoBehaviour
    {
        private void Awake()
        {
            var architecture = new TapGame();
            architecture.Init();
        }
    }
}