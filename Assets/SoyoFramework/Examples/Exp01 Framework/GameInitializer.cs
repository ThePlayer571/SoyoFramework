using UnityEngine;

namespace SoyoFramework.Examples.Exp01_Framework
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