using System;
using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;

namespace SoyoFramework.Framework.Runtime.ProcedureKit.DataClasses
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ProcedureTagsAttribute : Attribute
    {
        public ProcedureTag[] Tags { get; }

        public ProcedureTagsAttribute(params ProcedureTag[] tags)
        {
            Tags = tags ?? Array.Empty<ProcedureTag>();
        }
    }
}