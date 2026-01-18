using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    public class UIRoot : MonoBehaviour
    {
        [field: SerializeField] [NotNull] public Camera UICamera { get; private set; }
        [field: SerializeField] [NotNull] public Canvas Canvas { get; private set; }
        [SerializeField] internal UIManager UIManager;

        private Dictionary<string, Transform> _layerTransforms = new();

        /// <summary>
        /// 获取指定层的Transform
        /// </summary>
        public Transform GetLayerTransform(string layerKey)
        {
            return _layerTransforms.GetValueOrDefault(layerKey);
        }

        internal void InitLayers(IReadOnlyList<string> layerKeys)
        {
            // 查找Canvas下的默认层
            var defaultLayers = Enumerable.Range(0, Canvas.transform.childCount)
                .Select(i => Canvas.transform.GetChild(i))
                .ToList();

            // 初始化层级容器
            foreach (var layerKey in layerKeys)
            {
                if (_layerTransforms.ContainsKey(layerKey))
                    continue;

                var existingLayer = defaultLayers.FirstOrDefault(layer => layer.name == layerKey);
                if (existingLayer != null)
                {
                    // 已有该层：直接使用
                    _layerTransforms[layerKey] = existingLayer;
                }
                else
                {
                    // 没有该层：创建默认层
                    var newLayer = new GameObject(layerKey, typeof(RectTransform));
                    newLayer.transform.SetParent(Canvas.transform, false);
                    var rectTransform = newLayer.transform as RectTransform;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;

                    newLayer.name = layerKey;
                    _layerTransforms[layerKey] = newLayer.transform;
                }
            }
        }
    }
}