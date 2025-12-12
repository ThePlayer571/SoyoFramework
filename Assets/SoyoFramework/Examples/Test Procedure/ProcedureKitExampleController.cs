using System;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;
using UnityEngine;

namespace SoyoFramework.Examples.Test_Procedure
{
    public class ProcedureKitExampleController : MonoBehaviour
    {
        #region 示例方法

        // 1. 基础用法
        private async UniTask RunExample_BasicUsage()
        {
            "创建流程管理器".LogInfo();
            IProcedureManager procedureManager = ProcedureManager.CreateInstance();
            $"流程管理器创建完成，当前流程：{procedureManager.CurrentProcedure}".LogInfo();

            // 订阅ProcedureA的事件
            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterNormal,
                _ => { "切换至ProcedureA，阶段：EnterNormal".LogInfo(); });

            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterLate,
                _ => { "切换至ProcedureA，阶段：EnterLate".LogInfo(); });

            await procedureManager.ChangeProcedure(ProcedureId.ProcedureA);

            $"流程切换完成，当前流程：{procedureManager.CurrentProcedure}".LogInfo();
        }

        // 2. 标签功能
        private async UniTask RunExample_TagFunctionality()
        {
            IProcedureManager procedureManager = ProcedureManager.CreateInstance();
            $"初始流程：{procedureManager.CurrentProcedure}".LogInfo();

            bool hasTagX = procedureManager.HasTag(ProcedureId.Entrance, ProcedureTag.TagX);
            bool hasTagY = procedureManager.HasTag(ProcedureId.ProcedureA, ProcedureTag.TagY);
            bool currentHasTagY = procedureManager.CurrentHasTag(ProcedureTag.TagY);

            $"Entrance是否有TagX：{hasTagX}".LogInfo();
            $"ProcedureA是否有TagY：{hasTagY}".LogInfo();
            $"当前流程（{procedureManager.CurrentProcedure}）是否有TagY：{currentHasTagY}".LogInfo();

            await procedureManager.ChangeProcedure(ProcedureId.ProcedureA);
            currentHasTagY = procedureManager.CurrentHasTag(ProcedureTag.TagY);
            $"切换后，当前流程（{procedureManager.CurrentProcedure}）是否有TagY：{currentHasTagY}".LogInfo();

            var allTags = procedureManager.GetTags(procedureManager.CurrentProcedure);
            $"当前流程 {procedureManager.CurrentProcedure} 的所有标签: {string.Join(",", allTags)}".LogInfo();
        }

        // 3. 延时功能
        private async UniTask RunExample_DelayFeature()
        {
            IProcedureManager procedureManager = ProcedureManager.CreateInstance();

            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterEarly,
                _ => { "ProcedureA进入阶段EnterEarly".LogInfo(); });
            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterNormal,
                _ => { "ProcedureA进入阶段EnterNormal".LogInfo(); });

            // 在EnterEarly和EnterNormal之间添加自定义延时
            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterEarly, _ =>
            {
                $"将在进入EnterNormal前等待2秒".LogInfo();
                procedureManager.AddAwait(UniTask.Delay(TimeSpan.FromSeconds(2)));
            });

            await procedureManager.ChangeProcedure(ProcedureId.ProcedureA);

            "延时功能示例完成".LogInfo();
        }

        // 4. 切换拦截（流程切换检测模式）
        private async UniTask RunExample_Intercept()
        {
            IProcedureManager procedureManager = ProcedureManager.CreateInstance();
            $"默认的检查模式：{procedureManager.CheckMode}，此时不会阻断，只会提示".LogInfo();

            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterNormal,
                _ => { "进入ProcedureA的EnterNormal阶段".LogInfo(); });

            procedureManager.RegisterProcedure(ProcedureId.Entrance, ProcedureChangeStage.EnterNormal,
                _ => { "进入Entrance的EnterNormal阶段".LogInfo(); });

            procedureManager.CheckMode = ProcedureCheckMode.ErrorAndStop;
            $"切换至阻断模式：{procedureManager.CheckMode}".LogInfo();

            // 允许的切换 Entrance->ProcedureA
            await procedureManager.ChangeProcedure(ProcedureId.ProcedureA);

            // 不允许的切换 ProcedureA->Entrance（会被阻断）
            await procedureManager.ChangeProcedure(ProcedureId.Entrance);

            $"当前流程：{procedureManager.CurrentProcedure}".LogInfo();

            procedureManager.CheckMode = ProcedureCheckMode.Warning;
            $"切换回提示模式：{procedureManager.CheckMode}".LogInfo();

            await procedureManager.ChangeProcedure(ProcedureId.Entrance);
            $"当前流程：{procedureManager.CurrentProcedure}".LogInfo();

            "切换拦截示例结束".LogInfo();
        }

        // 5. 传参功能
        private async UniTask RunExample_PassingParameters()
        {
            IProcedureManager procedureManager = ProcedureManager.CreateInstance();

            procedureManager.RegisterProcedure(ProcedureId.ProcedureA, ProcedureChangeStage.EnterNormal, info =>
            {
                var userName = info.Paras.GetPara<string>("userName");
                $"进入ProcedureA的EnterNormal阶段，收到参数userName：{userName}".LogInfo();
            });

            await procedureManager.ChangeProcedure(ProcedureId.ProcedureA, ("userName", "Tony"));

            "传参功能示例结束".LogInfo();
        }

        #endregion

        #region IMGUI示例界面

        // 防止重复按钮点击
        private bool _runningTask = false;

        private void OnGUI()
        {
            int width = 250, height = 38, margin = 12, y = 10;

            GUI.enabled = !_runningTask;
            if (GUI.Button(new Rect(10, y, width, height), "1. 基础用法"))
            {
                RunButtonClicked(RunExample_BasicUsage);
            }

            y += height + margin;

            if (GUI.Button(new Rect(10, y, width, height), "2. 标签功能"))
            {
                RunButtonClicked(RunExample_TagFunctionality);
            }

            y += height + margin;

            if (GUI.Button(new Rect(10, y, width, height), "3. 延时功能"))
            {
                RunButtonClicked(RunExample_DelayFeature);
            }

            y += height + margin;

            if (GUI.Button(new Rect(10, y, width, height), "4. 切换拦截"))
            {
                RunButtonClicked(RunExample_Intercept);
            }

            y += height + margin;

            if (GUI.Button(new Rect(10, y, width, height), "5. 传参功能"))
            {
                RunButtonClicked(RunExample_PassingParameters);
            }

            y += height + margin;

            GUI.enabled = true;
            if (_runningTask)
            {
                GUI.Label(new Rect(10, y, 480, 24), "运行中...请稍候。");
            }
        }

        // 按钮安全触发
        private void RunButtonClicked(Func<UniTask> exampleMethod)
        {
            if (_runningTask) return;
            _runningTask = true;
            exampleMethod().ContinueWith(() => _runningTask = false).Forget();
        }

        #endregion
    }
}