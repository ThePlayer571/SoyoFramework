using UnityEngine;

namespace SoyoFramework
{
    public class MonoVController : MonoBehaviour, IMonoVController
    {
        public virtual IArchitecture RelyingArchitecture => GlobalArchitecture.Instance;
    }
}