using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core
{
    [BelongsToLayer("System + Model + Service")]
    public interface IProcedureManager<TProcedureId, TTagId>
    {
        // [Model]
        // 数据
        TProcedureId CurrentProcedure { get; }
        ProcedureCheckMode CheckMode { get; set; }
        bool IsChangingProcedure { get; }

        // 标签
        bool HasTag(TProcedureId procedureId, TTagId tag);
        IReadOnlyCollection<TTagId> GetTags(TProcedureId procedureId);
        bool CurrentHasTag(TTagId tag);
        IReadOnlyCollection<TTagId> GetCurrentTags();

        // [SendEvent]
        // 流程切换
        UniTask ChangeProcedure(TProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras);
        UniTask ChangeProcedure(TProcedureId procedureId, params (string, object)[] paras);

        // [RegisterEvent]
        // 流程订阅
        IUnRegister Register(TProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback);

        EasyEvent<TProcedureId, ProcedureChangeStage> OnProcedureChange { get; }

        // [Service]
        // 延迟切换
        void AddAwait(UniTask task);
    }
}