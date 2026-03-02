namespace SoyoFramework.Framework.Runtime.Core.Layers
{
    public abstract class AbstractModule : IModule
    {
        #region 接口实现

        IArchitecture ICanAttachToArchitecture.AttachedArchitecture { get; set; }
        bool ICanInitByArchitecture.PreInitialized => _preInitialized;
        bool ICanInitByArchitecture.Initialized => _initialized;

        void ICanInitByArchitecture.Init()
        {
            if (!_preInitialized)
            {
                ((ICanInitByArchitecture)this).PreInit();
            }

            OnInit();
            _initialized = true;
        }

        void ICanInitByArchitecture.PreInit()
        {
            OnPreInit();
            _preInitialized = true;
        }

        void ICanInitByArchitecture.Deinit()
        {
            OnDeinit();
        }

        #endregion

        private bool _preInitialized;
        private bool _initialized;


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