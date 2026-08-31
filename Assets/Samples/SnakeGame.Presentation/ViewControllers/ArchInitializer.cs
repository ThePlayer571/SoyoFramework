using SoyoFramework.Samples.SnakeGame.Backend;
using UnityEngine;

namespace SoyoFramework.Samples.SnakeGame.Presentation.ViewControllers
{
    [DefaultExecutionOrder(-1)]
    public class ArchInitializer : MonoBehaviour
    {
        private void Awake()
        {
            TSG.Init();
        }
    }
}