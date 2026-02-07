using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.ToolKits.Runtime.RandomKit.RandomPool
{
    public interface IRandomPool<TEntry, TOutput>
        where TEntry : class, IRandomPoolEntry<TOutput>
        where TOutput : class
    {
        /// <summary>
        /// 是否已完成（价值耗尽或无可用项）
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// 剩余的Value
        /// </summary>
        float RemainingValue { get; }

        /// <summary>
        /// 计算权重时，会根据此计算出一个新的权重值（为null时不会计算）。
        /// 这是一个纯函数
        /// </summary>
        WeightModifierDelegate<TEntry, TOutput> WeightModifier { get; set; }

        /// <summary>
        /// 随机生成一个输出
        /// </summary>
        /// <returns>随机的输出。如果没有满足条件的输出，会返回null</returns>
        TOutput Generate();

        /// <summary>
        /// 随机生成多个输出
        /// </summary>
        /// <param name="count">生成数量</param>
        /// <returns>随机的输出，List和List内的元素皆不会为null。如果没有满足条件的输出，List.Count会小于count</returns>
        List<TOutput> Generate(int count);


        /// <summary>
        /// 随机生成输出，直到IsFinished
        /// </summary>
        /// <returns>随机的输出。List和List内的元素皆不会为null</returns>
        List<TOutput> GenerateAllRemaining();
    }

    public delegate float WeightModifierDelegate<TEntry, TOutput>(TEntry entry)
        where TEntry : class, IRandomPoolEntry<TOutput>
        where TOutput : class;

    public class RandomPool<TEntry, TOutput> : IRandomPool<TEntry, TOutput>
        where TEntry : class, IRandomPoolEntry<TOutput>
        where TOutput : class
    {
        #region 接口实现 ISingleUseRandomPool

        public bool IsFinished => _isFinished;
        public float RemainingValue => _remainingValue;

        public WeightModifierDelegate<TEntry, TOutput> WeightModifier
        {
            get => _weightModifier;
            set
            {
                //bad: ValidateEntries函数使用了_weightModifier，因此引入了old变量缓存，导致可读性下降
                var old = _weightModifier;
                _weightModifier = value;
                if (!ValidateEntries())
                {
                    "设置WeightModifier失败".LogError();
                    _weightModifier = old;
                    return;
                }

                UpdateTotalWeightCache();
            }
        }

        public TOutput Generate()
        {
            if (_isFinished)
            {
                "在RandomPool完成后调用了Generate".LogError();
                return null;
            }

            return ReturnRandomOutput();
        }

        public List<TOutput> Generate(int count)
        {
            if (count <= 0)
            {
                "count的值必须大于0".LogError();
                return new List<TOutput>();
            }

            if (_isFinished)
            {
                "在RandomPool完成后调用了Generate".LogError();
                return new List<TOutput>();
            }

            var result = new List<TOutput>(count);
            for (int i = 0; i < count; i++)
            {
                var randomOutput = ReturnRandomOutput();
                if (randomOutput != null)
                {
                    result.Add(randomOutput);
                }

                if (_isFinished)
                {
                    break;
                }
            }

            return result;
        }

        public List<TOutput> GenerateAllRemaining()
        {
            if (_isFinished)
            {
                "在RandomPool完成后调用了GenerateAllRemaining".LogError();
                return new List<TOutput>();
            }

            var result = new List<TOutput>();
            while (!_isFinished)
            {
                var randomOutput = ReturnRandomOutput();
                if (randomOutput != null)
                {
                    result.Add(randomOutput);
                }
            }

            return result;
        }

        #endregion

        #region 构造

        public RandomPool(IEnumerable<TEntry> entries, float value, DeterministicRandom random)
        {
            _isFinished = false;
            _remainingValue = value;
            _availableEntries = entries.ToList();
            _random = random;
            _maxAllowedValueOffset = value * MaxAllowableValueOffsetRatio;

            Init();
        }

        private void Init()
        {
            if (!ValidateEntries())
            {
                "RandomPool初始化失败".LogError();
                _isFinished = true;
                return;
            }

            UpdateTotalWeightCache();
            UpdateIsFinishedStatus();
        }

        #endregion

        // 只读
        private const float MaxAllowableValueOffsetRatio = 0.05f;
        private readonly float _maxAllowedValueOffset;

        // 变量
        private bool _isFinished;
        private float _remainingValue;
        private readonly List<TEntry> _availableEntries;
        private readonly DeterministicRandom _random;
        private WeightModifierDelegate<TEntry, TOutput> _weightModifier;
        private readonly Dictionary<TEntry, int> _entryReturnCounts = new();

        private float _totalWeightCache = -1f;

        #region 内部函数（数据层面）

        private void UpdateTotalWeightCache()
        {
            _totalWeightCache = _availableEntries.Sum(GetWeight);
        }

        private void UpdateIsFinishedStatus()
        {
            _isFinished = _availableEntries.Count == 0 || _remainingValue < _maxAllowedValueOffset;
        }

        private bool ValidateEntries()
        {
            foreach (var entry in _availableEntries)
            {
                if (entry == null)
                {
                    $"Entry不能为空".LogError();
                    return false;
                }

                var weight = WeightModifier?.Invoke(entry) ?? entry.Weight;

                if (weight < 0f || float.IsNaN(weight) || float.IsInfinity(weight))
                {
                    $"Entry的Weight必须 >= 0且为有限值。Actual={weight}, Entry={entry}".LogError();
                    return false;
                }
            }

            return true;
        }

        private float GetWeight(TEntry entry)
        {
            if (_weightModifier != null)
            {
                return _weightModifier(entry);
            }
            else
            {
                return entry.Weight;
            }
        }

        #endregion

        #region 内部函数（操作）

        private TOutput ReturnRandomOutput()
        {
            if (_isFinished) return null;

            while (!_isFinished)
            {
                var selectedEntry = SelectRandomEntry();
                if (selectedEntry == null) break;

                if (CanReturnEntry(selectedEntry))
                {
                    // 消耗价值
                    ConsumeValue(selectedEntry.Value);

                    // 计算最多可返回次数
                    if (selectedEntry.MaxReturnCount > 0)
                    {
                        _entryReturnCounts[selectedEntry] = _entryReturnCounts.GetValueOrDefault(selectedEntry, 0) + 1;

                        if (_entryReturnCounts[selectedEntry] >= selectedEntry.MaxReturnCount)
                        {
                            RemoveEntry(selectedEntry);
                        }
                    }

                    return selectedEntry.Output;
                }
                else
                {
                    RemoveEntry(selectedEntry);
                }
            }

            return null;
        }


        private void ConsumeValue(float value)
        {
            _remainingValue -= value;

            UpdateIsFinishedStatus();
        }

        private void RemoveEntry(TEntry entry)
        {
            _availableEntries.Remove(entry);

            UpdateIsFinishedStatus();
            UpdateTotalWeightCache();
        }

        [Pure]
        private TEntry SelectRandomEntry()
        {
            if (_totalWeightCache <= 0f) return null;

            var totalWeight = _totalWeightCache;
            var chosenWeight = _random.Value * totalWeight;

            var currentWeight = 0f;
            foreach (var entry in _availableEntries)
            {
                currentWeight += GetWeight(entry);
                if (currentWeight >= chosenWeight)
                {
                    return entry;
                }
            }

            "意外地未能选中任何Entry，可能是weight的缓存问题或浮点数误差导致。默认返回最后一个值".LogWarning();
            return _availableEntries.Last();
        }

        [Pure]
        private bool CanReturnEntry(TEntry entry)
        {
            return _remainingValue - entry.Value > -_maxAllowedValueOffset;
        }

        #endregion
    }
}