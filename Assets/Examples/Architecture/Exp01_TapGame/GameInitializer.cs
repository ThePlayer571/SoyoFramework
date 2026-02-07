using UnityEngine;

namespace SoyoFramework.Framework.Examples.Exp01_TapGame
{
    public class GameInitializer : MonoBehaviour
    {
        private void Awake()
        {
            TapGame.Init();
        }
    }
}