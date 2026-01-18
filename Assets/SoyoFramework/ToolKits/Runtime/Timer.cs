using System;
using SoyoFramework.Framework.Runtime.Utils;

namespace SoyoFramework.ToolKits.Runtime
{
    /// <summary>
    /// 计时器接口，提供基本的计时功能
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 计时器的总持续时间（秒）
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 已经过的时间（秒）
        /// </summary>
        public float ElapsedTime { get; set; }

        /// <summary>
        /// 剩余时间（秒），等于 Duration - ElapsedTime
        /// </summary>
        public float RemainingTime { get; set; }

        /// <summary>
        /// 计时器是否已完成（ElapsedTime >= Duration）
        /// </summary>
        public bool IsComplete { get; }

        /// <summary>
        /// 是否刚刚完成计时
        /// 在计时结束后的第一次Update时为true，之后为false
        /// 用于检测计时器完成的瞬间，避免重复触发逻辑
        /// </summary>
        public bool IsJustCompleted { get; }

        /// <summary>
        /// 更新计时器
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public void Update(float deltaTime);

        /// <summary>
        /// 重置计时器到初始状态
        /// </summary>
        public void Reset();

        /// <summary>
        /// 计时器完成状态变化事件
        /// 当计时器从未完成变为完成，或从完成变为未完成时触发
        /// </summary>
        public EasyEvent<bool> OnCompleteChange { get; }

        /// <summary>
        /// 获取计时器完成的比率（0~1，ElapsedTime / Duration，Duration为0时返回0）
        /// </summary>
        public float CompletionRatio { get; }
    }

    /// <summary>
    /// 计时器实现类
    /// </summary>
    public class Timer : ITimer
    {
        #region 接口实现

        public float Duration { get; set; }

        public float ElapsedTime
        {
            get => _elapsedTime;
            set => SetElapsedTime(value);
        }

        public float RemainingTime
        {
            get => Math.Max(0f, Duration - ElapsedTime);
            set => SetElapsedTime(Duration - Math.Max(0f, value));
        }

        public bool IsComplete
        {
            get => _isComplete;
            private set
            {
                if (value != _isComplete)
                {
                    _isComplete = value;
                    OnCompleteChange.Trigger(value);
                }
            }
        }

        public bool IsJustCompleted { get; private set; }

        public float CompletionRatio => Duration > 0f ? Math.Min(1f, ElapsedTime / Duration) : 0f;

        public void Update(float deltaTime)
        {
            // 如果已经完成，清除"刚刚完成"标记并返回
            if (IsComplete)
            {
                IsJustCompleted = false;
                return;
            }

            SetElapsedTime(ElapsedTime + deltaTime);
        }

        public void Reset()
        {
            SetElapsedTime(0f);
            IsJustCompleted = false;
        }

        public EasyEvent<bool> OnCompleteChange { get; } = new EasyEvent<bool>();

        #endregion

        private float _elapsedTime;
        private bool _isComplete;

        private void SetElapsedTime(float value)
        {
            _elapsedTime = value;
            // 检查是否需要清除完成标记
            if (IsComplete && _elapsedTime < Duration)
            {
                IsComplete = false;
                IsJustCompleted = false;
            }
            // 如果未完成但超过Duration，也要刷新状态
            else if (!IsComplete && _elapsedTime >= Duration)
            {
                IsComplete = true;
                IsJustCompleted = true;
            }
        }

        /// <summary>
        /// 创建一个新的计时器
        /// </summary>
        /// <param name="duration">计时持续时间（秒）</param>
        public Timer(float duration)
        {
            Duration = duration;
            Reset();
        }
    }
}