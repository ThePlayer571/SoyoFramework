using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses;

namespace SoyoFramework.Examples.Test_Procedure
{
    public enum ProcedureId
    {
        Entrance = 0,
        ProcedureA = 1,
        ProcedureB = 2,
    }

    public enum ProcedureTag
    {
        TagX = 0,
        TagY = 1,
    }

    public class ProcedureConfig : ProcedureConfig<ProcedureId, ProcedureTag>
    {
        public override ProcedureId InitialProcedure { get; } = ProcedureId.Entrance;

        [NotNull]
        public override Dictionary<ProcedureId, MetaData> MetaDatas { get; } = new()
        {
            {
                ProcedureId.Entrance,
                new MetaData(
                    new ProcedureTag[] { ProcedureTag.TagX },
                    new ProcedureId[] { }
                )
            },
            {
                ProcedureId.ProcedureA,
                new MetaData(
                    new ProcedureTag[] { ProcedureTag.TagY },
                    new ProcedureId[] { ProcedureId.Entrance }
                )
            },
            {
                ProcedureId.ProcedureB,
                new MetaData(
                    new ProcedureTag[] { },
                    new ProcedureId[] { ProcedureId.Entrance, ProcedureId.ProcedureA }
                )
            },
        };
    }
}