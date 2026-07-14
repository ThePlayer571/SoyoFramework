using System;
using System.Collections.Generic;
using SoyoFramework.Utils;
using SoyoFramework.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework.ToolKits
{
    public class TriggerDetector : MonoBehaviour
    {
        #region 对外接口

        // 基础数据获取
        public bool HasTarget => _targetCount > 0 || DebugAlwaysReturnHasTarget;
        public int TargetCount => _targetCount;

        // 更改记录模式
        public bool RecordTarget
        {
            get => _recordTarget;
            set
            {
                if (_recordTarget == value) return;
                _recordTarget = value;
                OnRecordTargetChanged();
            }
        }

        /// <summary>
        /// 判别函数，只记录返回值为true的target
        /// </summary>
        public Func<Collider2D, bool>? TargetPredicate
        {
            get => _targetPredicate;
            set
            {
                _targetPredicate = value;
                if (Application.isPlaying)
                {
                    RefreshTargetPredicate();
                }
            }
        }

        // 排序比较器，可动态设置
        public IComparer<Collider2D>? Comparer
        {
            get => _comparer;
            set
            {
                _comparer = value;
                _sortDirty = true;
            }
        }

        public IEnumerable<Collider2D> DetectedTargets
        {
            get
            {
                if (!RecordTarget)
                {
                    "在未开启RecordTarget时尝试获取DetectedTargets".LogError();
                    return new List<Collider2D>();
                }

                // 懒加载排序：只在需要时排序
                if (Comparer != null && _sortDirty)
                {
                    _detectedTargets.Sort(Comparer);
                    _sortDirty = false;
                }

                return _detectedTargets.AsReadOnly();
            }
        }

        public Collider2D? FirstTarget
        {
            get
            {
                if (!RecordTarget || _detectedTargets.Count == 0)
                    return null;

                // 懒加载排序：只在需要时排序
                if (Comparer != null && _sortDirty)
                {
                    _detectedTargets.Sort(Comparer);
                    _sortDirty = false;
                }

                return _detectedTargets[0];
            }
        }

        // 事件
        public readonly EasyEvent<int> OnTargetCountChanged = new();
        public readonly EasyEvent<Collider2D> OnTargetEnter = new();
        public readonly EasyEvent<Collider2D> OnTargetExit = new();

        #endregion

        [SerializeField] private bool _recordTarget;
        private bool _recordTargetPrev;
        private Func<Collider2D, bool>? _targetPredicate;
        private IComparer<Collider2D>? _comparer;
        private readonly List<Collider2D> _detectedTargets = new();
        private int _targetCount = 0;
        private bool _sortDirty;

        [SerializeField] public bool DebugAlwaysReturnHasTarget = false; // 调试选项，始终返回有目标

        private void Awake()
        {
            _recordTargetPrev = _recordTarget;
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;

            _detectedTargets.Clear();

            if (_targetCount > 0)
            {
                _targetCount = 0;
                OnTargetCountChanged.Trigger(0);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying && _recordTarget != _recordTargetPrev)
            {
                OnRecordTargetChanged();
            }

            _recordTargetPrev = _recordTarget;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                // 防止检测器上有多个Trigger Collider时重复添加
                if (RecordTarget && _detectedTargets.Contains(other))
                    return;

                _targetCount++;
                if (RecordTarget)
                {
                    _detectedTargets.Add(other);
                }

                _sortDirty = true;
                OnTargetEnter.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                // 防止检测器上有多个Trigger Collider时重复移除
                if (RecordTarget && !_detectedTargets.Contains(other))
                    return;

                _targetCount = Mathf.Max(0, _targetCount - 1);
                if (RecordTarget)
                {
                    _detectedTargets.Remove(other);
                }

                _sortDirty = true;
                OnTargetExit.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        /// <summary>
        /// 在 RecordTarget 变更时同步内部状态
        /// </summary>
        private void OnRecordTargetChanged()
        {
            if (!Application.isPlaying) return;

            if (!_recordTarget)
            {
                // 关闭记录时清除已追踪的目标，避免下次开启时残留
                _detectedTargets.Clear();
            }
            // 开启记录时不回溯填充，后续进入/退出事件会自然填充
        }

        /// <summary>
        /// 在 TargetPredicate 变更时重新评估已有目标，移除不符合新条件的条目
        /// </summary>
        private void RefreshTargetPredicate()
        {
            if (_targetPredicate == null) return;

            int removedCount = _detectedTargets.RemoveAll(collider => !_targetPredicate(collider));
            if (removedCount > 0)
            {
                _targetCount = _detectedTargets.Count;
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }
    }
}