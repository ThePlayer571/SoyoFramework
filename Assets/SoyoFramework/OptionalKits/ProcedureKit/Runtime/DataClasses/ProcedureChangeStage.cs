namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses
{
    public enum ProcedureChangeStage
    {
        BeforeEnter,
        EnterEarly,
        EnterNormal,
        EnterLate,
        LeaveEarly,
        LeaveNormal,
        LeaveLate
    }

    public static class ProcedureChangeStageExtension
    {
        public static bool IsEnter(this ProcedureChangeStage self)
        {
            return self is ProcedureChangeStage.EnterEarly or ProcedureChangeStage.EnterNormal
                or ProcedureChangeStage.EnterLate;
        }
        
        public static bool IsLeave(this ProcedureChangeStage self)
        {
            return self is ProcedureChangeStage.LeaveEarly or ProcedureChangeStage.LeaveNormal
                or ProcedureChangeStage.LeaveLate;
        }
    }
}