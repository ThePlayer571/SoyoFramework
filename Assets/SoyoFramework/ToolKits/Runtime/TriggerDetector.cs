using System;
using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Utils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using UnityEngine;

namespace SoyoFramework.ToolKits.Runtime
{
    public class TriggerDetector : MonoBehaviour
    {
        #region Public

        // 基础数据获取
        public bool HasTarget => _targetCount > 0 || DebugAlwaysReturnHasTarget;
        public int TargetCount => _targetCount;

        // 更改记录模式
        public bool RecordTarget { get; set; }

        /// <summary>
        /// 判别函数，只记录返回值为true的target
        /// </summary>
        public Func<Collider2D, bool> TargetPredicate { get; set; }

        // 排序比较器，可动态设置
        public IComparer<Collider2D> Comparer { get; set; }

        public IEnumerable<Collider2D> DetectedTargets
        {
            get
            {
                if (!RecordTarget)
                {
                    "在未开启RecordTarget时尝试获取DetectedTargets".LogError();
                    return new List<Collider2D>();
                }

                // 懒加载排序：只在访问时排序
                if (Comparer != null)
                {
                    _detectedTargets.Sort(Comparer);
                }

                return _detectedTargets;
            }
        }

        public Collider2D FirstTarget
        {
            get
            {
                if (!RecordTarget || _detectedTargets.Count == 0)
                    return null;

                // 懒加载排序：只在访问时排序
                if (Comparer != null)
                {
                    _detectedTargets.Sort(Comparer);
                }

                return _detectedTargets[0];
            }
        }

        // 事件
        public readonly EasyEvent<int> OnTargetCountChanged = new();
        public readonly EasyEvent<Collider2D> OnTargetEnter = new();
        public readonly EasyEvent<Collider2D> OnTargetExit = new();

        #endregion

        private List<Collider2D> _detectedTargets;
        private int _targetCount = 0;

        [SerializeField] public bool DebugAlwaysReturnHasTarget = false; // 调试选项，始终返回有目标

        private void Awake()
        {
            _detectedTargets = new List<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount++;
                if (RecordTarget)
                {
                    _detectedTargets.Add(other);
                }

                OnTargetEnter.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount--;
                if (RecordTarget)
                {
                    _detectedTargets.Remove(other);
                }

                OnTargetExit.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }
    }
}