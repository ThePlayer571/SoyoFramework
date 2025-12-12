using System;
using SoyoFramework.Framework.Runtime.UsefulTools;

namespace SoyoFramework.Framework.Runtime.Core.SuperLayers
{
    /// <summary>
    /// 标记不符合架构规范的层
    /// </summary>
    public class SuperLayerAttribute : Attribute
    {
    }

    /// <summary>
    /// 标记架构设计缺陷，给出重构建议
    /// </summary>
    public class HasBetterArchAttribute : SuperLayerAttribute
    {
        public string Description { get; }

        public HasBetterArchAttribute(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// 标记该类属于哪些层，通常用于单例。有此特性的类无需重构
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BelongsToLayerAttribute : SuperLayerAttribute
    {
        public BelongsToLayerAttribute(params LayerType[] layers)
        {
        }
    }

    public enum LayerType
    {
        System,
        Model,
        Service,
        ViewController
    }

    [Experimental]
    public class ForConvenienceInjectionAttribute : SuperLayerAttribute
    {
        public ForConvenienceInjectionAttribute(string description)
        {
        }
    }
}