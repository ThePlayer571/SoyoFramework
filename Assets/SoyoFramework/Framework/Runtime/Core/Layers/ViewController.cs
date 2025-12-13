using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class ViewController : AbstractModule, IViewController
    {
    }

    public abstract class MonoVController : MonoBehaviour, IMonoVController
    {
        public IArchitecture RelyingArchitecture => ArchitectureHelper.DefaultArchitecture;
    }
}