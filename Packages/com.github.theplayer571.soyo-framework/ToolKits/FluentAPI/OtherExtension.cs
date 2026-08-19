using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace SoyoFramework.ToolKits.FluentAPI
{
    public static class ListExtensions
    {
        /// <summary>
        /// 移除并返回列表末尾的元素（类似Python的pop()）
        /// </summary>
        public static T Pop<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new InvalidOperationException("Cannot pop from empty list.");

            int lastIndex = list.Count - 1;
            T item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }

        /// <summary>
        /// 移除并返回指定索引处的元素（类似Python的pop(index)）
        /// </summary>
        public static T Pop<T>(this List<T> list, int index)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index));

            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }


    public static class Vector2IntExtensions
    {
        /// <summary>
        /// 返回上方（y+step，相同x）的新Vector2Int
        /// </summary>
        [Pure]
        public static Vector2Int Up(this Vector2Int v, int step = 1)
        {
            return new Vector2Int(v.x, v.y + step);
        }

        /// <summary>
        /// 返回下方（y-step，相同x）的新Vector2Int
        /// </summary>
        [Pure]
        public static Vector2Int Down(this Vector2Int v, int step = 1)
        {
            return new Vector2Int(v.x, v.y - step);
        }

        /// <summary>
        /// 返回左方（x-step，相同y）的新Vector2Int
        /// </summary>
        [Pure]
        public static Vector2Int Left(this Vector2Int v, int step = 1)
        {
            return new Vector2Int(v.x - step, v.y);
        }

        /// <summary>
        /// 返回右方（x+step，相同y）的新Vector2Int
        /// </summary>
        [Pure]
        public static Vector2Int Right(this Vector2Int v, int step = 1)
        {
            return new Vector2Int(v.x + step, v.y);
        }
    }

    public static class Vector2Extensions
    {
        /// <summary>
        /// 返回 vector 顺时针旋转 angleDegrees 度后的新 Vector2。
        /// </summary>
        /// <param name="vector">原始向量</param>
        /// <param name="angleDegrees">旋转角度（度），正值为顺时针</param>
        /// <returns>旋转后的 Vector2</returns>
        [Pure]
        public static Vector2 Rotate(this Vector2 vector, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(
                vector.x * cos + vector.y * sin,
                -vector.x * sin + vector.y * cos
            );
        }
    }
}