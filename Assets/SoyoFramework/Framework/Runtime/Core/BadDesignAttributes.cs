using System;

namespace SoyoFramework.Framework.Runtime.Core
{
    /// <summary>
    /// 标记设计缺陷
    /// </summary>
    public class BadDesignAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记当前层级具有超越当前层级的职能
    /// </summary>
    public class SuperLayerAttribute : BadDesignAttribute
    {
        public string Description { get; }

        public SuperLayerAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 当为了写代码方便而违反规范时使用。标记设计缺陷，给出重构建议
    /// </summary>
    public class HasBetterArchAttribute : BadDesignAttribute
    {
        public string Description { get; }

        public HasBetterArchAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 为了将违反框架的内容整合进框架时使用。标记该类应该属于哪些层（即便代码实际上不是这么写的），通常用于单例。有此特性标记代表无需重构
    /// </summary>
    public class BelongsToLayerAttribute : BadDesignAttribute
    {
        public string Description { get; }

        public BelongsToLayerAttribute(string description)
        {
            Description = description;
        }
    }
}