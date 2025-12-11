using System;

namespace SoyoFramework.Framework.Runtime.Core.SuperLayers
{
    public class HasBetterArchAttribute : Attribute
    {
        public string Description { get; }

        public HasBetterArchAttribute(string description)
        {
            Description = description;
        }
    }
}