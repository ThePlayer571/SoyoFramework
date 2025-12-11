using System;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;

namespace SoyoFramework.Framework.Runtime.ProcedureKit.DataClasses
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AllowedPreviousProceduresAttribute : Attribute
    {
        public ProcedureId[] AllowedPrevious { get; }

        public AllowedPreviousProceduresAttribute(params ProcedureId[] allowedPrevious)
        {
            AllowedPrevious = allowedPrevious ?? Array.Empty<ProcedureId>();
        }
    }
}