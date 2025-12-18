using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.UITests
{
    public class UIPageTestManager : MonoBehaviour
    {
        // 引用
        [Header("引用")] [SerializeField] private Transform DesignUIRoot;
        [SerializeField] private Camera MainCamera;

        // 测试配置
        [Header("测试配置")] [SerializeField] private string OpenPageName;
        [SerializeField] private UISettings UISettings;

        // 调试区域
        [Header("调试区域")] [SerializeReference] List<IUIContext> RegisteredContexts;

        private void Start()
        {
            UniTask.Void(async () =>
            {
                // 隐去做设计的页面
                DesignUIRoot.gameObject.SetActive(false);

                // 初始化
                UIKit.Init(UISettings);
                var uiPage = await UIKit.OpenPageAsync<UIPage>(OpenPageName);

                // 绑定到MainCamera
                UIKit.PutUICameraIntoStack(MainCamera);

                // 利用反射获取UIPage的IOCContainer，并将Context的引用赋值到RegisteredContexts
                var iocField = typeof(UIPage).GetField("_iocContainer", BindingFlags.NonPublic | BindingFlags.Instance);
                var iocContainer = iocField?.GetValue(uiPage);
                if (iocContainer != null)
                {
                    var getAllMethod = iocContainer.GetType().GetMethod("GetAll").MakeGenericMethod(typeof(IUIContext));
                    var result = getAllMethod.Invoke(iocContainer, null) as IEnumerable<IUIContext>;
                    RegisteredContexts = result?.ToList();
                }
                else
                {
                    RegisteredContexts = new List<IUIContext>();
                }
            });
        }
    }
}