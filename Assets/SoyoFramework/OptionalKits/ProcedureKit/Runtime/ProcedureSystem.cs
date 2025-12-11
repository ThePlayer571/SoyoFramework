using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core.Layers;
using SoyoFramework.Framework.Runtime.Core.SuperLayers;
using SoyoFramework.Framework.Runtime.ProcedureKit;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;
using SoyoFramework.Framework.Runtime.UsefulTools;
using SoyoFramework.Scripts.ToolKits.PhaseKit;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime
{
    public interface IProcedureSystem : ISuperSystem, IProcedureManager
    {
    }

    public class ProcedureSystem : AbstractSystem, IProcedureSystem
    {
        private IProcedureManager _procedureManager;

        #region System初始化

        protected override void OnPreInit()
        {
            _procedureManager = new ProcedureManager();
        }

        #endregion

        #region IProcedureManager接口实现

        public ProcedureId CurrentProcedure => _procedureManager.CurrentProcedure;

        public ProcedureCheckMode CheckMode
        {
            get => _procedureManager.CheckMode;
            set => _procedureManager.CheckMode = value;
        }

        public bool HasTag(ProcedureId procedureId, ProcedureTag tag)
            => _procedureManager.HasTag(procedureId, tag);

        public IReadOnlyList<ProcedureTag> GetTags(ProcedureId procedureId)
            => _procedureManager.GetTags(procedureId);

        public bool CurrentHasTag(ProcedureTag tag)
            => _procedureManager.CurrentHasTag(tag);

        public IReadOnlyList<ProcedureTag> GetCurrentTags()
            => _procedureManager.GetCurrentTags();

        public UniTask ChangeProcedure(ProcedureId procedureId, ProcedureChangeInfo.ProcedureChangeParas paras)
            => _procedureManager.ChangeProcedure(procedureId, paras);

        public UniTask ChangeProcedure(ProcedureId procedureId, params (string, object)[] paras)
            => _procedureManager.ChangeProcedure(procedureId, paras);

        public IUnRegister RegisterProcedure(ProcedureId procedureId, ProcedureChangeStage stage,
            Action<ProcedureChangeInfo> callback)
            => _procedureManager.RegisterProcedure(procedureId, stage, callback);

        public void AddAwait(UniTask task)
            => _procedureManager.AddAwait(task);

        #endregion
    }
}