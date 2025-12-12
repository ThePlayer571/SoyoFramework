using SoyoFramework.OptionalKits.ProcedureKit.Runtime.Core;

namespace SoyoFramework.Examples.Test_Procedure
{
    public interface IProcedureManager : IProcedureManager<ProcedureId, ProcedureTag>
    {
    }

    public class ProcedureManager : ProcedureManager<ProcedureId, ProcedureTag>, IProcedureManager
    {
        private ProcedureManager(ProcedureConfig config) : base(config)
        {
        }

        public static IProcedureManager CreateInstance()
        {
            var config = new ProcedureConfig();
            return new ProcedureManager(config);
        }
    }
}