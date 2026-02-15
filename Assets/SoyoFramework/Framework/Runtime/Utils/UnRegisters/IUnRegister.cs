namespace SoyoFramework.Framework.Runtime.Utils.UnRegisters
{
    public interface IUnRegister
    {
        void UnRegister();
        IUnRegister Combine(IUnRegister other);
    }
}