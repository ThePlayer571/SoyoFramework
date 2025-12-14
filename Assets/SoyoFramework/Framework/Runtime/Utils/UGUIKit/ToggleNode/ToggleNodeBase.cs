using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.Framework.Runtime.Utils.UGUIKit.ToggleNode
{
    /// <summary>
    /// Toggle 节点基类，支持泛型 Key 类型
    /// </summary>
    /// <typeparam name="TKey">Key 的类型</typeparam>
    [ExecuteAlways]
    public abstract class ToggleNodeBase<TKey> : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] protected List<ToggleNodeInfo<TKey>> _toggleNodeInfos = new();

        #endregion

        #region 数据修改方法 | 事件

        public void Add(TKey key, Toggle toggle)
        {
            if (HasKey(key))
            {
                Debug.LogWarning($"[ToggleNode] 已存在相同 Key: {key}，添加失败", this);
                return;
            }

            var newInfo = new ToggleNodeInfo<TKey>
            {
                Key = key,
                Toggle = toggle
            };
            _toggleNodeInfos.Add(newInfo);
            OnToggleAdded.Trigger(newInfo);
            OnToggleInfoChanged.Trigger();
        }

        public void Remove(TKey key)
        {
            var infoToRemove = _toggleNodeInfos.FirstOrDefault(info => KeyEquals(info.Key, key));
            if (infoToRemove.Toggle != null)
            {
                _toggleNodeInfos.Remove(infoToRemove);
                OnToggleRemoved.Trigger(infoToRemove);
                OnToggleInfoChanged.Trigger();
            }
        }

        public void Remove(Toggle toggle)
        {
            var infoToRemove = _toggleNodeInfos.FirstOrDefault(info => info.Toggle == toggle);
            if (infoToRemove.Toggle != null)
            {
                _toggleNodeInfos.Remove(infoToRemove);
                OnToggleRemoved.Trigger(infoToRemove);
                OnToggleInfoChanged.Trigger();
            }
        }

        public EasyEvent<ToggleNodeInfo<TKey>> OnToggleAdded { get; } = new();
        public EasyEvent<ToggleNodeInfo<TKey>> OnToggleRemoved { get; } = new();
        public EasyEvent OnToggleInfoChanged { get; } = new();

        #endregion

        #region 数据读取方法

        /// <summary>
        /// 根据 Key 获取 Toggle
        /// </summary>
        public Toggle GetToggleByKey(TKey key)
        {
            return _toggleNodeInfos
                .Where(info => KeyEquals(info.Key, key))
                .Select(info => info.Toggle)
                .FirstOrDefault();
        }

        /// <summary>
        /// 根据 Toggle 获取 Key
        /// </summary>
        public TKey GetKeyByToggle(Toggle toggle)
        {
            return _toggleNodeInfos
                .Where(info => info.Toggle == toggle)
                .Select(info => info.Key)
                .FirstOrDefault();
        }

        /// <summary>
        /// 检查 Key 是否存在
        /// </summary>
        public bool HasKey(TKey key)
        {
            return _toggleNodeInfos.Any(info => KeyEquals(info.Key, key));
        }

        /// <summary>
        /// 尝试获取 Toggle，返回是否成功
        /// </summary>
        public bool TryGetToggle(TKey key, out Toggle toggle)
        {
            toggle = GetToggleByKey(key);
            return toggle != null;
        }

        public int Count => _toggleNodeInfos.Count;
        public IEnumerable<TKey> Keys => _toggleNodeInfos.Select(info => info.Key);
        public IEnumerable<Toggle> Toggles => _toggleNodeInfos.Select(info => info.Toggle);

        #endregion

        #region 虚方法 - 可重写

        /// <summary>
        /// Key 比较方法，子类可重写以自定义比较逻辑
        /// </summary>
        protected virtual bool KeyEquals(TKey a, TKey b)
        {
            return EqualityComparer<TKey>.Default.Equals(a, b);
        }

        /// <summary>
        /// 验证 Key 是否有效，子类可重写
        /// </summary>
        protected virtual bool IsValidKey(TKey key)
        {
            return key != null;
        }

        #endregion

        #region Editor 支持

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            // 检查重复 Key
            var duplicateKeys = _toggleNodeInfos
                .Where(info => IsValidKey(info.Key))
                .GroupBy(info => info.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateKeys.Count > 0)
            {
                Debug.LogWarning($"[ToggleNode] 存在重复的 Key: {string.Join(", ", duplicateKeys)}", this);
            }

            OnToggleInfoChanged.Trigger();
        }
#endif

        #endregion
    }

    /// <summary>
    /// Toggle 节点信息，支持泛型 Key
    /// </summary>
    [Serializable]
    public struct ToggleNodeInfo<TKey> : IEquatable<ToggleNodeInfo<TKey>>
    {
        public TKey Key;
        public Toggle Toggle;

        public bool Equals(ToggleNodeInfo<TKey> other)
        {
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && Toggle == other.Toggle;
        }

        public override bool Equals(object obj)
        {
            return obj is ToggleNodeInfo<TKey> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Toggle);
        }

        public static bool operator ==(ToggleNodeInfo<TKey> left, ToggleNodeInfo<TKey> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ToggleNodeInfo<TKey> left, ToggleNodeInfo<TKey> right)
        {
            return !left.Equals(right);
        }
    }
}