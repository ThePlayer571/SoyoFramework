using System;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime
{
    /// <summary>
    /// 枚举排序类型
    /// </summary>
    public enum EnumSortType
    {
        /// <summary>按字母顺序排序</summary>
        Alphabetical = 0,

        /// <summary>保持默认顺序</summary>
        Default = 1
    }

    /// <summary>
    /// 自定义枚举弹窗特性
    /// 用于在 Inspector 中显示可搜索、可排序的枚举选择器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CustomEnumPopupAttribute : PropertyAttribute
    {
        /// <summary>在 Inspector 中显示的属性名称</summary>
        public string DisplayName { get; private set; }

        /// <summary>枚举值的排序类型</summary>
        public EnumSortType SortType { get; private set; }

        /// <summary>
        /// 使用默认设置创建特性
        /// </summary>
        public CustomEnumPopupAttribute()
        {
            DisplayName = null;
            SortType = EnumSortType.Default;
        }

        /// <summary>
        /// 指定排序类型创建特性
        /// </summary>
        /// <param name="sortType">排序类型</param>
        public CustomEnumPopupAttribute(EnumSortType sortType)
        {
            DisplayName = null;
            SortType = sortType;
        }

        /// <summary>
        /// 指定显示名称创建特性
        /// </summary>
        /// <param name="displayName">自定义显示名称</param>
        public CustomEnumPopupAttribute(string displayName)
        {
            DisplayName = displayName;
            SortType = EnumSortType.Default;
        }

        /// <summary>
        /// 指定显示名称和排序类型创建特性
        /// </summary>
        /// <param name="displayName">自定义显示名称</param>
        /// <param name="sortType">排序类型</param>
        public CustomEnumPopupAttribute(string displayName, EnumSortType sortType)
        {
            DisplayName = displayName;
            SortType = sortType;
        }
    }
}