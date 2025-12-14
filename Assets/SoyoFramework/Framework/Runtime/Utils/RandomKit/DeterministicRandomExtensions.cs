using System;
using System.Collections.Generic;

namespace SoyoFramework.Framework.Runtime.Utils.RandomKit
{
    public static class DeterministicRandomExtensions
    {
        /// <summary>
        /// 从集合中随机选取指定数量的元素（无重复）
        /// </summary>
        public static IEnumerable<T> RandomSubset<T>(this IEnumerable<T> source, DeterministicRandom random, int count)
        {
            return random.RandomSubset(source, count);
        }

        /// <summary>
        /// 从集合中随机选择一个元素
        /// </summary>
        public static T RandomChoose<T>(this IEnumerable<T> source, DeterministicRandom random)
        {
            return random.RandomChoose(source);
        }

        /// <summary>
        /// 从集合中根据权重随机选择一个元素
        /// </summary>
        public static T RandomChoose<T>(this IEnumerable<T> source, DeterministicRandom random, Func<T, float> weightSelector)
        {
            return random.RandomChoose(source, weightSelector);
        }

        /// <summary>
        /// Fisher-Yates洗牌算法
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, DeterministicRandom random)
        {
            return random.Shuffle(source);
        }
        
        /// <summary>
        /// Fisher-Yates洗牌算法
        /// </summary>
        public static IList<T> Shuffle<T>(this IList<T> source, DeterministicRandom random)
        {
            return random.Shuffle(source);
        }
        
        /// <summary>
        /// 从集合中随机移除并返回一个元素
        /// </summary>
        public static T RandomPop<T>(this IList<T> source, DeterministicRandom random)
        {
            return random.RandomPop(source);
        }
    }
}

