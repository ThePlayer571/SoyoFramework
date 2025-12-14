using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEditor;
using UnityEngine;

namespace SoyoFramework.Framework.Editor
{
    /// <summary>
    /// 自定义枚举弹窗窗口
    /// 支持搜索、排序、显示枚举值
    /// </summary>
    public class CustomEnumPopupWindow : EditorWindow
    {
        private SerializedProperty _property;
        private Type _enumType;
        private EnumSortType _sortType;

        private string _searchText = "";
        private Vector2 _scrollPosition;
        private List<EnumEntry> _allEntries;
        private List<EnumEntry> _filteredEntries;
        private int _selectedIndex = -1;
        private int _keyboardIndex = -1;

        private GUIStyle _itemStyle;
        private GUIStyle _selectedItemStyle;
        private GUIStyle _searchFieldStyle;
        private bool _stylesInitialized = false;

        private const float ITEM_HEIGHT = 20f;
        private const float WINDOW_WIDTH = 250f;
        private const float MAX_WINDOW_HEIGHT = 300f;
        private const float SEARCH_FIELD_HEIGHT = 22f;
        private const float PADDING = 4f;

        /// <summary>
        /// 枚举条目数据
        /// </summary>
        private class EnumEntry
        {
            public string Name;
            public int Value;
            public int OriginalIndex;

            public EnumEntry(string name, int value, int originalIndex)
            {
                Name = name;
                Value = value;
                OriginalIndex = originalIndex;
            }
        }

        /// <summary>
        /// 显示枚举弹窗
        /// </summary>
        public static void Show(Rect buttonRect, SerializedProperty property, Type enumType, EnumSortType sortType)
        {
            // 创建窗口
            CustomEnumPopupWindow window = CreateInstance<CustomEnumPopupWindow>();
            window._property = property;
            window._enumType = enumType;
            window._sortType = sortType;
            window._selectedIndex = property.enumValueIndex;
            window._keyboardIndex = property.enumValueIndex;

            // 初始化枚举条目
            window.InitializeEntries();

            // 计算窗口大小和位置
            float height = Mathf.Min(
                SEARCH_FIELD_HEIGHT + PADDING * 2 + window._allEntries.Count * ITEM_HEIGHT + PADDING,
                MAX_WINDOW_HEIGHT
            );

            // 转换为屏幕坐标
            Vector2 screenPos = GUIUtility.GUIToScreenPoint(new Vector2(buttonRect.x, buttonRect.yMax));
            Rect windowRect = new Rect(screenPos.x, screenPos.y, WINDOW_WIDTH, height);

            window.ShowAsDropDown(windowRect, new Vector2(WINDOW_WIDTH, height));
            window.Focus();
        }

        private void InitializeEntries()
        {
            _allEntries = new List<EnumEntry>();

            string[] names = _property.enumDisplayNames;
            Array values = Enum.GetValues(_enumType);

            for (int i = 0; i < names.Length; i++)
            {
                int value = (int)values.GetValue(i);
                _allEntries.Add(new EnumEntry(names[i], value, i));
            }

            // 根据排序类型排序
            if (_sortType == EnumSortType.Alphabetical)
            {
                _allEntries = _allEntries.OrderBy(e => e.Name).ToList();
            }

            UpdateFilteredEntries();
        }

        private void UpdateFilteredEntries()
        {
            if (string.IsNullOrEmpty(_searchText))
            {
                _filteredEntries = new List<EnumEntry>(_allEntries);
            }
            else
            {
                string lowerSearch = _searchText.ToLower();
                _filteredEntries = _allEntries
                    .Where(e => e.Name.ToLower().Contains(lowerSearch))
                    .ToList();
            }

            // 重置键盘索引
            if (_filteredEntries.Count > 0)
            {
                // 尝试保持选中当前值
                int currentIndex = _filteredEntries.FindIndex(e => e.OriginalIndex == _selectedIndex);
                _keyboardIndex = currentIndex >= 0 ? currentIndex : 0;
            }
            else
            {
                _keyboardIndex = -1;
            }
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized) return;

            // 普通项样式
            _itemStyle = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(8, 8, 2, 2),
                alignment = TextAnchor.MiddleLeft,
                fixedHeight = ITEM_HEIGHT
            };

            // 选中项样式
            _selectedItemStyle = new GUIStyle(_itemStyle);

            if (EditorGUIUtility.isProSkin)
            {
                // Professional 皮肤
                _selectedItemStyle.normal.background = MakeColorTexture(new Color(0.24f, 0.49f, 0.91f, 1f));
                _selectedItemStyle.normal.textColor = Color.white;
            }
            else
            {
                // Personal 皮肤
                _selectedItemStyle.normal.background = MakeColorTexture(new Color(0.24f, 0.49f, 0.91f, 1f));
                _selectedItemStyle.normal.textColor = Color.white;
            }

            _searchFieldStyle = new GUIStyle(EditorStyles.toolbarSearchField)
            {
                fixedHeight = SEARCH_FIELD_HEIGHT - 4
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            InitializeStyles();

            // 处理键盘事件
            HandleKeyboardInput();

            // 搜索框
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.SetNextControlName("SearchField");
            string newSearchText = EditorGUILayout.TextField(_searchText, _searchFieldStyle,
                GUILayout.Height(SEARCH_FIELD_HEIGHT - 4));

            if (newSearchText != _searchText)
            {
                _searchText = newSearchText;
                UpdateFilteredEntries();
            }

            EditorGUILayout.EndHorizontal();

            // 自动聚焦搜索框
            EditorGUI.FocusTextInControl("SearchField");

            // 分隔线
            DrawSeparator();

            // 枚举列表
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (_filteredEntries.Count == 0)
            {
                EditorGUILayout.LabelField("没有匹配的结果", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < _filteredEntries.Count; i++)
                {
                    EnumEntry entry = _filteredEntries[i];
                    bool isSelected = entry.OriginalIndex == _selectedIndex;
                    bool isKeyboardFocused = i == _keyboardIndex;

                    Rect itemRect = EditorGUILayout.GetControlRect(false, ITEM_HEIGHT);

                    // 绘制背景
                    GUIStyle style = (isSelected || isKeyboardFocused) ? _selectedItemStyle : _itemStyle;

                    // 绘制项目
                    string displayText = $"{entry.Name}  <color=#888888>({entry.Value})</color>";

                    // 创建富文本样式
                    GUIStyle richStyle = new GUIStyle(style)
                    {
                        richText = true
                    };

                    if (GUI.Button(itemRect, displayText, richStyle))
                    {
                        SelectEntry(entry);
                    }

                    // 显示勾选标记
                    if (isSelected)
                    {
                        Rect checkRect = new Rect(itemRect.xMax - 20, itemRect.y, 20, itemRect.height);
                        EditorGUI.LabelField(checkRect, "✓", new GUIStyle(EditorStyles.label)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            normal =
                            {
                                textColor = isKeyboardFocused
                                    ? Color.white
                                    : (EditorGUIUtility.isProSkin ? Color.green : new Color(0, 0.6f, 0))
                            }
                        });
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            // 点击窗口外部关闭
            if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
            {
                Close();
            }
        }

        private void HandleKeyboardInput()
        {
            Event e = Event.current;

            if (e.type != EventType.KeyDown) return;

            switch (e.keyCode)
            {
                case KeyCode.DownArrow:
                    if (_filteredEntries.Count > 0)
                    {
                        _keyboardIndex = Mathf.Min(_keyboardIndex + 1, _filteredEntries.Count - 1);
                        ScrollToIndex(_keyboardIndex);
                        e.Use();
                    }

                    break;

                case KeyCode.UpArrow:
                    if (_filteredEntries.Count > 0)
                    {
                        _keyboardIndex = Mathf.Max(_keyboardIndex - 1, 0);
                        ScrollToIndex(_keyboardIndex);
                        e.Use();
                    }

                    break;

                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (_keyboardIndex >= 0 && _keyboardIndex < _filteredEntries.Count)
                    {
                        SelectEntry(_filteredEntries[_keyboardIndex]);
                        e.Use();
                    }

                    break;

                case KeyCode.Escape:
                    Close();
                    e.Use();
                    break;
            }
        }

        private void ScrollToIndex(int index)
        {
            if (index < 0 || _filteredEntries.Count == 0) return;

            float itemTop = index * ITEM_HEIGHT;
            float itemBottom = itemTop + ITEM_HEIGHT;
            float viewHeight = position.height - SEARCH_FIELD_HEIGHT - PADDING * 2;

            if (itemTop < _scrollPosition.y)
            {
                _scrollPosition.y = itemTop;
            }
            else if (itemBottom > _scrollPosition.y + viewHeight)
            {
                _scrollPosition.y = itemBottom - viewHeight;
            }

            Repaint();
        }

        private void SelectEntry(EnumEntry entry)
        {
            _property.enumValueIndex = entry.OriginalIndex;
            _property.serializedObject.ApplyModifiedProperties();
            Close();
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin
                ? new Color(0.1f, 0.1f, 0.1f)
                : new Color(0.6f, 0.6f, 0.6f));
        }

        private void OnLostFocus()
        {
            // 失去焦点时关闭窗口
            Close();
        }
    }
}