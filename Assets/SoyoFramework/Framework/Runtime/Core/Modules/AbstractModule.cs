namespace SoyoFramework.Framework.Runtime.Core.Modules
{
    public abstract class AbstractModule : IModule
    {
        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }

        public bool PreInitialized { get; private set; }
        public bool Initialized { get; private set; }

        void ICanInit.Init()
        {
            if (!PreInitialized)
            {
                ((ICanInit)this).PreInit();
            }

            OnInit();
            Initialized = true;
        }

        void ICanInit.PreInit()
        {
            OnPreInit();
            PreInitialized = true;
        }

        void ICanInit.Deinit()
        {
            OnDeinit();
        }

        protected virtual void OnPreInit()
        {
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDeinit()
        {
        }
    }
}