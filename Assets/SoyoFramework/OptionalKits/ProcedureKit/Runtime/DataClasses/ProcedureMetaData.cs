using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;

namespace SoyoFramework.Framework.Runtime.ProcedureKit.DataClasses
{
    public class ProcedureMetaData
    {
        public IReadOnlyList<ProcedureTag> Tags;
        public IReadOnlyList<ProcedureId> AllowedPreviousProcedures;
    }
}