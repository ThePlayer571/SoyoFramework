using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core
{
    public class MonoVController : MonoBehaviour, IMonoVController
    {
        public virtual IArchitecture RelyingArchitecture => GlobalArchitecture.Instance;
    }
}