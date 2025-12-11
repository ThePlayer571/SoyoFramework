using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core.SuperLayers;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;
using SoyoFramework.Framework.Runtime.UsefulTools;
using SoyoFramework.Scripts.ToolKits.PhaseKit;

namespace SoyoFramework.Framework.Runtime.ProcedureKit
{
    public interface IProcedureModel
    {
        // 数据
        ProcedureId CurrentProcedure { get; }
        ProcedureCheckMode CheckMode { get; set; }

        // 标签
        bool HasTag(ProcedureId procedureId, ProcedureTag tag);
        IReadOnlyList<ProcedureTag> GetTags(ProcedureId procedureId);
        bool CurrentHasTag(ProcedureTag tag);
        IReadOnlyList<ProcedureTag> GetCurrentTags();
    }

    public interface IProcedureEventSender
    {
        // 流程切换
        UniTask ChangeProcedure(ProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras);
        UniTask ChangeProcedure(ProcedureId procedureId, params (string, object)[] paras);
    }

    public interface IProcedureEventReceiver
    {
        // 流程订阅
        IUnRegister RegisterProcedure(ProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback);
    }

    public interface IProcedureService
    {
        // 延迟切换
        void AddAwait(UniTask task);
    }

    public interface IProcedureManager :
        IProcedureModel, IProcedureEventSender, IProcedureEventReceiver, IProcedureService
    {
    }
}