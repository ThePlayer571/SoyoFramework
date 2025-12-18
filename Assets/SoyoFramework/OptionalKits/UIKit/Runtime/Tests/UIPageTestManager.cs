using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Tests
{
    public class UIPageTestManager : MonoBehaviour
    {
        // 引用
        [Header("引用")] [SerializeField] private UIRoot TestUIRoot;

        // 测试配置
        [Header("测试配置")] [SerializeField] private string OpenPageName;
        [SerializeField] private UISettings UISettings;

        // 调试区域
        [Header("调试区域")] [SerializeReference] List<IUIContext> RegisteredContexts;

        private void Start()
        {
            UniTask.Void(async () =>
            {
                TestUIRoot.gameObject.SetActive(false);

                UIKit.Init(UISettings);
                var uiPage = await UIKit.OpenPageAsync<UIPage>(OpenPageName);

                RegisteredContexts = uiPage.AllContexts.ToList();
            });
        }
    }
}