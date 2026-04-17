using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core
{
    public interface IProcedureModel<TProcedureId, TTagId>
    {
        // 数据
        TProcedureId CurrentProcedure { get; }
        bool IsChangingProcedure { get; }
        
        // 切换规则
        ProcedureCheckMode CheckMode { get; set; }
        bool HasChangeRule(TProcedureId previous, TProcedureId next);
        bool CurrentHasChangeRule(TProcedureId next);

        // 标签
        bool HasTag(TProcedureId procedureId, TTagId tag);
        IReadOnlyCollection<TTagId> GetTags(TProcedureId procedureId);
        bool CurrentHasTag(TTagId tag);
        IReadOnlyCollection<TTagId> GetCurrentTags();


        // 事件
        EasyEvent<TProcedureId, ProcedureChangeStage> OnProcedureChange { get; }

        IUnRegister Register(TProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback);
    }

    public interface IProcedureService<TProcedureId>
    {
        // 流程切换
        UniTask ChangeProcedure(TProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras);
        UniTask ChangeProcedure(TProcedureId procedureId, params (string, object)[] paras);

        void AddAwait(UniTask task);
    }

    public interface IProcedureManager<TProcedureId, TTagId> :
        IProcedureModel<TProcedureId, TTagId>,
        IProcedureService<TProcedureId>
    {
    }
}