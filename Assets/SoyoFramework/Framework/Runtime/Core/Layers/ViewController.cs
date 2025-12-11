using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class ViewController : MonoBehaviour, IViewController
    {
        public IArchitecture RelyingArchitecture => ArchitectureHelper.DefaultArchitecture;
    }
}