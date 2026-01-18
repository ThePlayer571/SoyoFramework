using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Framework.Runtime.Core.CoreUtils;
using SoyoFramework.Framework.Runtime.Utils.LogKit;
using SoyoFramework.Framework.Runtime.Utils.UGUIKit.ToggleNode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoyoFramework.Framework.Runtime.Utils.UGUIKit
{
    /// <summary>
    /// 超出最大选择数时的处理策略
    /// </summary>
    public enum DeselectStrategy
    {
        /// <summary>
        /// 自动取消最早选择的 Toggle
        /// </summary>
        AutoDeselectOldest,

        /// <summary>
        /// 阻止新的选择
        /// </summary>
        PreventNewSelection
    }

    /// <summary>
    /// 允许选择多个的ToggleGroup
    /// 依赖ToggleNode实现，有很多方法都是在ToggleNode里的，比如AddToggle
    /// </summary>
    [ExecuteAlways]
    public abstract class MultiToggleGroupBase<TKey> : UIBehaviour
    {
        #region 序列化字段

        [SerializeField, Min(1), Tooltip("最大可选择的 Toggle 数量")]
        private int _maxSelectionCount = 1;

        [SerializeField, Tooltip("是否允许取消所有选择（当为 false 时至少保持一个选中）")]
        private bool _allowSwitchOff = true;

        [SerializeField, Tooltip("当超出最大选择数时的处理策略")]
        private DeselectStrategy _deselectStrategy = DeselectStrategy.PreventNewSelection;

        #endregion

        #region 可用字段

        private ToggleNodeBase<TKey> _toggleNode;
        private readonly List<Toggle> _selectedToggles = new();
        private readonly Dictionary<Toggle, UnityEngine.Events.UnityAction<bool>> _toggleListeners = new();
        private bool _isUpdating;

        #endregion

        #region Public

        #region 数据配置

        /// <summary>
        /// 最大可选择数量
        /// </summary>
        public int MaxSelectionCount
        {
            get => _maxSelectionCount;
            set
            {
                _maxSelectionCount = Mathf.Max(1, value);
                EnforceMaxSelection();
            }
        }

        /// <summary>
        /// 是否允许取消所有选择
        /// </summary>
        public bool AllowSwitchOff
        {
            get => _allowSwitchOff;
            set => _allowSwitchOff = value;
        }

        /// <summary>
        /// 超出最大选择数时的处理策略
        /// </summary>
        public DeselectStrategy DeselectStrategy
        {
            get => _deselectStrategy;
            set => _deselectStrategy = value;
        }

        /// <summary>
        /// 关联的 ToggleNode
        /// </summary>
        public ToggleNodeBase<TKey> ToggleNode => _toggleNode ??= GetComponent<ToggleNodeBase<TKey>>();

        #endregion

        #region 数据读取

        /// <summary>
        /// 当前选中的 Toggle 数量
        /// </summary>
        public int SelectedCount => _selectedToggles.Count;

        /// <summary>
        /// 是否已达到最大选择数
        /// </summary>
        public bool IsMaxSelected => _selectedToggles.Count >= _maxSelectionCount;

        /// <summary>
        /// 获取所有被选中的 Toggle
        /// </summary>
        public IReadOnlyList<Toggle> GetSelectedToggles()
        {
            _selectedToggles.RemoveAll(t => t == null);
            return _selectedToggles.AsReadOnly();
        }

        /// <summary>
        /// 获取所有被选中的 Toggle 的 Key
        /// </summary>
        public IReadOnlyList<TKey> GetSelectedKeys()
        {
            // 将 Toggle 列表映射为 Key 并返回只读列表
            return GetSelectedToggles()
                .Select(t => ToggleNode.GetKeyByToggle(t))
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// 检查指定 Key 对应的 Toggle 是否被选中
        /// </summary>
        public bool IsSelectedByKey(TKey key)
        {
            var toggle = ToggleNode.GetToggleByKey(key);
            return toggle != null && _selectedToggles.Contains(toggle);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 当选择状态发生变化时触发
        /// </summary>
        public EasyEvent<TKey, bool> OnToggleValueChanged { get; } = new();

        /// <summary>
        /// 当选中列表发生变化时触发（改为以 TKey 列表的版本）
        /// </summary>
        public EasyEvent<IReadOnlyList<TKey>> OnSelectionChanged { get; } = new();

        #endregion

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _toggleNode = GetComponent<ToggleNodeBase<TKey>>();
            if (_toggleNode == null)
            {
                $"未找到 ToggleNode<{typeof(TKey)}> 组件，MultiToggleGroup<{typeof(TKey)}> 需要依赖 ToggleNode 工作。请确保该物体上存在 ToggleNode 组件。物体名称：{gameObject.name}"
                    .LogError();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (ToggleNode == null) return;

            RegisterAllToggles();
            SubscribeToToggleNode();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterAllToggles();
            UnsubscribeFromToggleNode();
        }

        #endregion

        #region 实用函数

        private void RegisterAllToggles()
        {
            _selectedToggles.Clear();
            _toggleListeners.Clear();

            foreach (var toggle in ToggleNode.Toggles)
            {
                RegisterToggle(toggle);
            }
        }

        private void RegisterToggle(Toggle toggle)
        {
            if (toggle == null || _toggleListeners.ContainsKey(toggle)) return;

            if (toggle.isOn)
            {
                if (_selectedToggles.Count < _maxSelectionCount)
                {
                    _selectedToggles.Add(toggle);
                }
                else
                {
                    toggle.isOn = false;
                }
            }

            void Listener(bool isOn) => OnToggleValueChangedInternal(toggle, isOn);
            toggle.onValueChanged.AddListener(Listener);
            _toggleListeners[toggle] = Listener;
        }

        private void UnregisterToggle(Toggle toggle)
        {
            if (toggle == null) return;

            if (_toggleListeners.TryGetValue(toggle, out var listener))
            {
                toggle.onValueChanged.RemoveListener(listener);
                _toggleListeners.Remove(toggle);
            }

            _selectedToggles.Remove(toggle);
        }

        private void UnregisterAllToggles()
        {
            foreach (var kvp in _toggleListeners)
            {
                kvp.Key?.onValueChanged.RemoveListener(kvp.Value);
            }

            _toggleListeners.Clear();
            _selectedToggles.Clear();
        }

        private void OnToggleValueChangedInternal(Toggle toggle, bool isOn)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                if (isOn)
                {
                    HandleToggleOn(toggle);
                }
                else
                {
                    HandleToggleOff(toggle);
                }

                OnToggleValueChanged.Trigger(ToggleNode.GetKeyByToggle(toggle), isOn);
                OnSelectionChanged.Trigger(GetSelectedKeys());
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void HandleToggleOn(Toggle toggle)
        {
            if (_selectedToggles.Contains(toggle)) return;

            if (_selectedToggles.Count >= _maxSelectionCount)
            {
                switch (_deselectStrategy)
                {
                    case DeselectStrategy.AutoDeselectOldest:
                        var oldest = _selectedToggles[0];
                        _selectedToggles.RemoveAt(0);
                        if (oldest != null)
                        {
                            oldest.isOn = false;
                        }

                        break;

                    case DeselectStrategy.PreventNewSelection:
                        toggle.isOn = false;
                        return;
                }
            }

            _selectedToggles.Add(toggle);
        }

        private void HandleToggleOff(Toggle toggle)
        {
            if (!_selectedToggles.Contains(toggle)) return;

            if (!_allowSwitchOff && _selectedToggles.Count <= 1)
            {
                toggle.isOn = true;
                return;
            }

            _selectedToggles.Remove(toggle);
        }

        private void EnforceMaxSelection()
        {
            while (_selectedToggles.Count > _maxSelectionCount)
            {
                var oldest = _selectedToggles[0];
                _selectedToggles.RemoveAt(0);
                if (oldest != null)
                {
                    oldest.isOn = false;
                }
            }

            OnSelectionChanged.Trigger(GetSelectedKeys());
        }

        private void SubscribeToToggleNode()
        {
            ToggleNode.OnToggleAdded.Register(info => RegisterToggle(info.Toggle));
            ToggleNode.OnToggleRemoved.Register(info => UnregisterToggle(info.Toggle));
        }

        private void UnsubscribeFromToggleNode()
        {
            ToggleNode.OnToggleAdded.UnRegister(info => RegisterToggle(info.Toggle));
            ToggleNode.OnToggleRemoved.UnRegister(info => UnregisterToggle(info.Toggle));
        }

        #endregion

        #region Editor Support

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _maxSelectionCount = Mathf.Max(1, _maxSelectionCount);

            if (Application.isPlaying && isActiveAndEnabled)
            {
                EnforceMaxSelection();
            }
        }
#endif

        #endregion
    }
}