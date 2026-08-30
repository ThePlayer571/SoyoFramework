using System;
using SoyoFramework.Utils;
using SoyoFramework.Utils.UnRegisters;
using UnityEngine;

namespace SoyoFramework
{
    public abstract class MonoVController : MonoBehaviour, IViewController
    {
        public virtual IArchitecture RelyingArchitecture
            => GlobalArchitecture.Instance ?? throw new InvalidOperationException(
                $"{nameof(GlobalArchitecture)} 未初始化。你应该调用对应 Architecture 的 Init 方法来初始化框架。\n" +
                $"如果你不打算使用 {nameof(GlobalArchitecture)}，请重写 {nameof(RelyingArchitecture)} 并返回正确的 {nameof(IArchitecture)} 实例。");
    }
}