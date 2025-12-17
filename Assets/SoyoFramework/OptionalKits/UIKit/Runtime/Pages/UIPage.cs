using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Core;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Pages
{
    public interface IUIViewHost
    {
        T GetContext<T>() where T : class, IUIContext;
        T GetModule<T>() where T : UIModule;
        void SubmitCommand(ICommand command);
        TResult SubmitCommand<TResult>(ICommand<TResult> command);
    }

    public abstract class UIPage : MonoBehaviour, IUIViewHost
    {
        #region Editor

        [SerializeField] private List<UIView> _views = new();

        #endregion

        #region Protected 子类可用

        /// <summary>
        /// 注册Context和Module
        /// </summary>
        protected abstract void Configure();

        protected void RegisterContext<T>(T context) where T : class, IUIContext
        {
            _contexts[typeof(T)] = context;
        }

        protected void RegisterModule(UIModule module)
        {
            _modules.Add(module);
        }

        protected abstract void OnInit();
        protected abstract void OnClose();

        #endregion


        private readonly Dictionary<Type, IUIContext> _contexts = new();
        private readonly List<UIModule> _modules = new();


        #region Lifecycle

        /// <summary>
        /// 初始化的入口，给外部用
        /// </summary>
        internal void Init()
        {
            // 注册Context和Module
            Configure();

            // 初始化
            OnInit();

            // 初始化View
            foreach (var view in _views)
            {
                view.Init(this);
            }
        }

        internal void Close()
        {
            // 关闭View
            foreach (var view in _views)
            {
                view.Close();
            }

            // 关闭
            OnClose();
        }

        #endregion

        #region IUIViewHost 接口实现

        public T GetContext<T>() where T : class, IUIContext
        {
            _contexts.TryGetValue(typeof(T), out var ctx);
            return ctx as T;
        }

        public T GetModule<T>() where T : UIModule
        {
            foreach (var module in _modules)
            {
                if (module is T typed)
                    return typed;
            }

            return null;
        }

        public abstract void SubmitCommand(ICommand command);

        public abstract TResult SubmitCommand<TResult>(ICommand<TResult> command);

        #endregion
    }
}