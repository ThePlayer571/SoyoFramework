using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.ToolKits.Runtime;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page
{
    public interface IUIViewHost : IGameObject
    {
        T GetContext<T>() where T : class, IUIContext;
        T GetModule<T>() where T : UIModule;
        void SubmitCommand(ICommand command);
        void SubmitCommand<TResult>(ICommand<TResult> command, out TResult result);
    }

    public abstract class UIPage : MonoBehaviour, IMonoVController, IUIViewHost
    {
        // Editor
        [SerializeField] internal List<UIView> _views = new();

        // 变量
        private readonly SimpleIOCContainer _iocContainer = new();
        private List<IUIPageLogic> _pageLogics = new();


        #region Protected 子类可用

        // 生命周期
        /// <summary>
        /// 配置阶段回调，注册Context和Module
        /// </summary>
        protected abstract void Configure();

        /// <summary>
        /// 初始化回调，一般用于绑定数据
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 关闭回调
        /// </summary>
        protected abstract void OnClose();

        /// <summary>
        /// 注册Context（请在Configure阶段调用）
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        protected void RegisterContext<T>(T context) where T : class, IUIContext
        {
            _iocContainer.Register<T>(context);
        }

        /// <summary>
        /// 注册Module（请在Configure阶段调用）
        /// </summary>
        /// <param name="module"></param>
        /// <typeparam name="T"></typeparam>
        protected void RegisterModule<T>(T module) where T : UIModule
        {
            _iocContainer.Register<T>(module);
        }

        /// <summary>
        /// 注册逻辑（请在Init阶段调用）（用于解决多Page的逻辑共享问题）
        /// </summary>
        /// <param name="pageLogic"></param>
        protected void RegisterLogic(IUIPageLogic pageLogic)
        {
            _pageLogics.Add(pageLogic);
        }

        /// <summary>
        /// 处理View发送的UICommand
        /// </summary>
        /// <param name="command">处理的Command</param>
        /// <returns>处理是否成功</returns>
        protected abstract bool HandleUICommand(UICommand command);

        #endregion

        #region 生命周期

        /// <summary>
        /// 初始化的入口，给外部用
        /// </summary>
        internal void Init(PageOpenSettings openSettings)
        {
            // 注册Context和Module
            Configure();

            // 初始化
            if (!openSettings.NotCallOnInit)
            {
                OnInit();
                foreach (var uiPageLogic in _pageLogics)
                {
                    uiPageLogic.OnInit();
                }
            }

            // 初始化View
            foreach (var view in _views)
            {
                try
                {
                    view.Init(this);
                }
                catch (Exception e)
                {
                    $"UIView: {view.GetType().Name} 初始化出错，异常信息：{e}".LogError();
                }
            }
        }

        internal void Close()
        {
            // 关闭View
            foreach (var view in _views)
            {
                try
                {
                    view.Close();
                }
                catch (Exception e)
                {
                    $"UIView: {view.GetType().Name} 关闭出错，异常信息：{e}".LogError();
                }
            }

            // 关闭
            OnClose();
            foreach (var uiPageLogic in _pageLogics)
            {
                uiPageLogic.OnClose();
            }
        }

        #endregion

        #region IUIViewHost 接口实现

        public T GetContext<T>() where T : class, IUIContext
        {
            return _iocContainer.Get<T>();
        }


        public T GetModule<T>() where T : UIModule
        {
            return _iocContainer.Get<T>();
        }

        public void SubmitCommand(ICommand command)
        {
            if (command is UICommand uiCommand)
            {
                // 通过注册的Logic处理
                foreach (var uiPageLogic in _pageLogics)
                {
                    if (uiPageLogic.TryHandleCommand(uiCommand))
                    {
                        return;
                    }
                }

                if (!HandleUICommand(uiCommand))
                {
                    $"未处理的UICommand: {uiCommand.GetType().Name}".LogError();
                }
            }
            else
            {
                if (RelyingArchitecture != null)
                {
                    // 非UICommand，发送至框架
                    this.SendCommand(command);
                }
                else
                {
                    // 无法处理的Command
                    $"无法处理的Command: {command.GetType().Name}，原因是当前Page没有依赖Architecture".LogError();
                }
            }
        }

        public void SubmitCommand<TResult>(ICommand<TResult> command, out TResult result)
        {
            this.SendCommand(command, out result);
        }

        #endregion

        public abstract IArchitecture RelyingArchitecture { get; }
    }
}