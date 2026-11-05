using System;

namespace SoyoFramework
{
    public abstract class AggregateRelationAttribute : Attribute
    {
        protected AggregateRelationAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes == null ? Array.Empty<Type>() : (Type[])targetTypes.Clone();
        }

        internal Type[] TargetTypes { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class ObservesAttribute : AggregateRelationAttribute
    {
        public ObservesAttribute(params Type[] targetTypes) : base(targetTypes) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class RequiresAttribute : AggregateRelationAttribute
    {
        public RequiresAttribute(params Type[] targetTypes) : base(targetTypes) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class ManagesAttribute : AggregateRelationAttribute
    {
        public ManagesAttribute(params Type[] targetTypes) : base(targetTypes) { }
    }
}
