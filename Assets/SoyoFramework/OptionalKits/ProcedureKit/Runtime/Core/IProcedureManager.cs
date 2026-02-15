using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.UnRegisters;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core
{
    public interface IProcedureModel<TProcedureId, TTagId>
    {
        // 数据
        TProcedureId CurrentProcedure { get; }
        ProcedureCheckMode CheckMode { get; set; }

        bool IsChangingProcedure { get; }

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