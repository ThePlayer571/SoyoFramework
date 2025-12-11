namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractModule : IModule
    {
        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }

        public bool PreInitialized { get; private set; }
        public bool Initialized { get; private set; }

        void ICanInitByArchitecture.Init()
        {
            if (!PreInitialized)
            {
                ((ICanInitByArchitecture)this).PreInit();
            }

            OnInit();
            Initialized = true;
        }

        void ICanInitByArchitecture.PreInit()
        {
            OnPreInit();
            PreInitialized = true;
        }

        void ICanInitByArchitecture.Deinit()
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