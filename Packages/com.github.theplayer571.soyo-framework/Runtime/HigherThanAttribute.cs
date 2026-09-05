using System;

namespace SoyoFramework
{
    /// <summary>
    /// 声明当前聚合在领域组织层级上高于指定的聚合 key。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HigherThanAttribute : Attribute
    {
        private readonly Type[] _lowerTypes;

        public HigherThanAttribute(params Type[] lowerTypes)
        {
            _lowerTypes = lowerTypes == null ? Array.Empty<Type>() : (Type[])lowerTypes.Clone();
        }

        internal Type[] LowerTypes => _lowerTypes;
    }
}
