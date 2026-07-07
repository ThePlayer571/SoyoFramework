using UnityEngine;

namespace SoyoFramework.Runtime
{
    public class MonoVController : MonoBehaviour, IMonoVController
    {
        public virtual IArchitecture RelyingArchitecture => GlobalArchitecture.Instance;
    }
}