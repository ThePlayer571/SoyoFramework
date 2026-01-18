using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;

namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractVController : AbstractModule, IVController
    {
    }

    public abstract class MonoVController : MonoBehaviour, IMonoVController
    {
        public virtual IArchitecture RelyingArchitecture => ArchitectureHelper.DefaultArchitecture;
    }

    /// <summary>
    /// 属于ViewController层，但是不与框架直接交互，而是通过依赖注入交互
    /// </summary>
    [Experimental]
    public abstract class MonoView : MonoBehaviour
    {
    }

    public interface IDefaultMonoVController : IMonoVController
    {
        IArchitecture ICanRelyOnArchitecture.RelyingArchitecture => ArchitectureHelper.DefaultArchitecture;
    }
}