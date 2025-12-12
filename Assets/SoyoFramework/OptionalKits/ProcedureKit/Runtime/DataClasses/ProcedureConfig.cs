using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses
{
    public abstract class ProcedureConfig<TProcedureId, TTagId>
    {
        public abstract TProcedureId InitialProcedure { get; }
        [NotNull] public abstract Dictionary<TProcedureId, MetaData> MetaDatas { get; }

        public class MetaData
        {
            public MetaData(IReadOnlyCollection<TTagId> tags,
                IReadOnlyCollection<TProcedureId> allowingPreviousProcedures)
            {
                Tags = tags ?? Array.Empty<TTagId>();
                AllowingPreviousProcedures = allowingPreviousProcedures ?? Array.Empty<TProcedureId>();
            }


            [NotNull] public IReadOnlyCollection<TTagId> Tags { get; }
            [NotNull] public IReadOnlyCollection<TProcedureId> AllowingPreviousProcedures { get; }
        }
    }
}