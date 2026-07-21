using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework.ToolKits.RandomKit
{
    public sealed class DeterministicRandom
    {
        # region 核心实现

        // 随机算法状态
        private ulong _state0;
        private ulong _state1;


        /// <summary>
        /// 生成下一个随机数（0 ~ UInt64.MaxValue）
        /// </summary>
        public ulong NextUnsigned()
        {
            ulong s1 = _state0;
            ulong s0 = _state1;
            _state0 = s0;
            s1 ^= s1 << 23;
            _state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return _state1 + s0;
        }

        #endregion

        #region 构造

        /// <summary>
        /// 使用指定种子初始化
        /// </summary>
        public DeterministicRandom(ulong seed)
        {
            // 使用SplitMix64算法初始化状态
            _state0 = SplitMix64(ref seed);
            _state1 = SplitMix64(ref seed);
        }

        /// <summary>
        /// 使用指定种子初始化
        /// </summary>
        /// <param name="seed"></param>
        public DeterministicRandom(int seed) : this((ulong)seed)
        {
        }

        /// <summary>
        /// 使用当前时间戳初始化
        /// </summary>
        public DeterministicRandom() : this((ulong)DateTime.Now.Ticks)
        {
        }

        /// <summary>
        /// 使用指定状态初始化
        /// </summary>
        /// <param name="state"></param>
        public DeterministicRandom(State state)
        {
            _state0 = state.state0;
            _state1 = state.state1;
        }

        #endregion

        #region 实用方法

        /// <summary>
        /// 生成区间内的整数 [min, max)
        /// </summary>
        public int Range(int min, int max)
        {
            if (min > max)
            {
                $"InvalidArgument: 参数的 min 大于 max。已通过交换二者的值处理".LogWarning();
                (min, max) = (max, min);
            }

            if (min == max)
            {
                "InvalidArgument: 参数 min 等于 max。已将 max += 1 处理".LogWarning();
                max++;
            }

            return (int)(Value * (max - min)) + min;
        }

        /// <summary>
        /// 生成区间内的浮点数 [min, max]
        /// </summary>
        public float Range(float min, float max)
        {
            if (min > max)
            {
                $"InvalidArgument: 参数的 min 大于 max。已通过交换二者的值处理".LogWarning();
                (min, max) = (max, min);
            }

            return (float)Value * (max - min) + min;
        }

        /// <summary>
        /// 生成区间内的浮点数 [0.0, 1.0)
        /// </summary>
        public double Value => (NextUnsigned() >> 11) * (1.0 / (1ul << 53));

        /// <summary>
        /// 生成随机布尔值
        /// </summary>
        public bool NextBool()
        {
            return (NextUnsigned() & 1) == 1;
        }

        /// <summary>
        /// 根据指定概率生成随机布尔值
        /// </summary>
        /// <param name="probability">返回true的概率，范围[0.0, 1.0]</param>
        /// <returns>根据概率生成的布尔值</returns>
        public bool NextBool(float probability)
        {
            if (probability < 0.0f || probability > 1.0f)
            {
                $"ArgumentOutOfRange: 概率值必须在0.0到1.0之间。当前值为 {probability}".LogError();
                probability = Mathf.Clamp(probability, 0.0f, 1.0f);
            }

            return Value < probability;
        }

        /// <summary>
        /// 从集合中随机选取指定数量的元素（无重复）
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="count">选取数量（需非负）</param>
        ///
        public IEnumerable<T> RandomSubset<T>(IEnumerable<T> source, int count)
        {
            if (count < 0)
            {
                $"ArgumentOutOfRangeException: 选取数量 count 为负（{count}），已替换为 0".LogWarning();
                count = 0;
            }

            var list = source.ToList();
            if (count == 0 || list.Count == 0)
                return Enumerable.Empty<T>();
            if (count >= list.Count)
                return Shuffle(list);

            // 部分洗牌算法（Fisher-Yates 优化版）
            for (int i = 0; i < count; i++)
            {
                int j = Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list.Take(count);
        }

        /// <summary>
        /// 从集合中随机选择一个元素
        /// </summary>
        public T RandomChoose<T>(IEnumerable<T> source)
        {
            if (source is not IReadOnlyList<T> list)
            {
                list = source.ToList();
            }

            if (list.Count == 0)
            {
                throw new InvalidOperationException("集合不能为空，无法选择元素");
            }

            return list[Range(0, list.Count)];
        }

        /// <summary>
        /// 从集合中根据权重随机选择一个元素
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="weightSelector">权重选择器，返回每个元素的权重（需非负）</param>
        /// <returns>根据权重随机选中的元素</returns>
        public T RandomChoose<T>(IEnumerable<T> source, Func<T, float> weightSelector)
        {
            if (source is not IReadOnlyList<T> list)
            {
                list = source.ToList();
            }

            if (list.Count == 0)
            {
                throw new InvalidOperationException("集合不能为空，无法选择元素");
            }

            float totalWeight = 0f;
            var weights = new List<float>(list.Count);
            foreach (var item in list)
            {
                float w = weightSelector(item);
                if (w < 0f)
                {
                    $"ArgumentException: 权重不能为负（{w}），已替换为 0".LogWarning();
                    w = 0f;
                }

                weights.Add(w);
                totalWeight += w;
            }

            if (totalWeight <= 0f)
            {
                "所有权重之和零，无法随机。已返回第一个值".LogError();
                return list[0];
            }

            float r = (float)Range(0, int.MaxValue) / int.MaxValue * totalWeight;
            float acc = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                acc += weights[i];
                if (r < acc)
                    return list[i];
            }

            // 理论上不会到这里
            return list[^1];
        }

        /// <summary>
        /// 从集合中随机移除并返回一个元素
        /// </summary>
        /// <param name="source">可变的集合（需支持按索引移除）</param>
        public T RandomPop<T>(IList<T> source)
        {
            if (source.Count == 0)
            {
                throw new InvalidOperationException("集合不能为空，无法选择元素");
            }

            int index = Range(0, source.Count);
            T item = source[index];
            source.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Fisher-Yates洗牌算法
        /// </summary>
        public IList<T> Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
        {
            var list = source.ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        /// <summary>
        /// 通过扰动因子（如时间戳、线程ID等）扰乱当前状态
        /// </summary>
        public void Scramble(ulong entropy)
        {
            // 使用扰动因子与当前状态结合
            ulong tmp = entropy;
            _state0 ^= SplitMix64(ref tmp);
            _state1 ^= SplitMix64(ref tmp);
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 当前随机状态
        /// </summary>
        public struct State : IEquatable<State>
        {
            public ulong state0;
            public ulong state1;

            public State(ulong seed)
            {
                state0 = SplitMix64(ref seed);
                state1 = SplitMix64(ref seed);
            }

            public bool Equals(State other)
            {
                return state0 == other.state0 && state1 == other.state1;
            }

            public override bool Equals(object? obj)
            {
                return obj is State other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(state0, state1);
            }

            public static bool operator ==(State left, State right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(State left, State right)
            {
                return !left.Equals(right);
            }
        }

        /// <summary>
        /// 保存当前状态
        /// </summary>
        public State SaveState()
        {
            return new State { state0 = _state0, state1 = _state1 };
        }

        /// <summary>
        /// 恢复先前状态
        /// </summary>
        public void RestoreState(State state)
        {
            _state0 = state.state0;
            _state1 = state.state1;
        }

        public void RestoreState(ulong seed)
        {
            RestoreState(new State(seed));
        }

        #endregion

        #region 私有方法

        private static ulong SplitMix64(ref ulong x)
        {
            ulong z = (x += 0x9E3779B97F4A7C15);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }

        #endregion
    }
}