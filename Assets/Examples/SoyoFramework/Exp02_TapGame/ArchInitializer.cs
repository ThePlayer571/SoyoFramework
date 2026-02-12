using UnityEngine;

namespace Examples.SoyoFramework.Exp02_TapGame
{
    public class ArchInitializer : MonoBehaviour
    {
        private void Awake()
        {
            TapGame.Init();
        }
    }
}