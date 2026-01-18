#if UNITY_EDITOR

using SoyoFramework.OptionalKits.UIKit.Runtime;
using SoyoFramework.OptionalKits.UIKit.Runtime.UITest;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SoyoFramework.OptionalKits.UIKit. Editor
{
    [CustomEditor(typeof(UIPageTestManager))]
    public class UIPageTestManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认编辑器
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            var manager = (UIPageTestManager)target;

            // 验证并比较 Canvas 设置
            ValidateAndCompareCanvasSettings(manager);
        }

        private void ValidateAndCompareCanvasSettings(UIPageTestManager manager)
        {
            // 获取 DesignUIRoot
            var designUIRootField = serializedObject.FindProperty("DesignUIRoot");
            var designUIRoot = designUIRootField.objectReferenceValue as Transform;

            // 获取 UISettings
            var uiSettingsField = serializedObject.FindProperty("UISettings");
            var uiSettings = uiSettingsField. objectReferenceValue as UISettings;

            // 边界情况检查
            if (designUIRoot == null)
            {
                EditorGUILayout.HelpBox("DesignUIRoot 未设置", MessageType.Error);
                return;
            }

            if (uiSettings == null)
            {
                EditorGUILayout.HelpBox("UISettings 未设置", MessageType.Error);
                return;
            }

            if (uiSettings.UIRoot == null)
            {
                EditorGUILayout.HelpBox("UISettings.UIRoot 未设置", MessageType.Error);
                return;
            }

            // 获取 DesignUIRoot 下的 Canvas 和 CanvasScaler（在子对象中查找）
            var designCanvas = designUIRoot.GetComponentInChildren<Canvas>();
            var designCanvasScaler = designUIRoot.GetComponentInChildren<CanvasScaler>();

            if (designCanvas == null)
            {
                EditorGUILayout.HelpBox("DesignUIRoot 下未找到 Canvas 组件", MessageType.Error);
                return;
            }

            if (designCanvasScaler == null)
            {
                EditorGUILayout.HelpBox("DesignUIRoot 下未找到 CanvasScaler 组件", MessageType. Error);
                return;
            }

            // 获取 UISettings. UIRoot 的 UIRoot 组件
            var uiRootComponent = uiSettings.UIRoot. GetComponent<UIRoot>();
            if (uiRootComponent == null)
            {
                EditorGUILayout.HelpBox("UISettings.UIRoot 上未找到 UIRoot 脚本", MessageType.Error);
                return;
            }

            var referenceCanvas = uiRootComponent.Canvas;
            var referenceCanvasScaler = uiRootComponent.CanvasScaler;

            if (referenceCanvas == null)
            {
                EditorGUILayout.HelpBox("UISettings.UIRoot 的 UIRoot 脚本中 Canvas 属性为空", MessageType.Error);
                return;
            }

            if (referenceCanvasScaler == null)
            {
                EditorGUILayout.HelpBox("UISettings.UIRoot 的 UIRoot 脚本中 CanvasScaler 属性为空", MessageType. Error);
                return;
            }

            // 比较 Canvas 和 CanvasScaler 设置
            bool isCanvasDifferent = IsCanvasDifferent(designCanvas, referenceCanvas);
            bool isCanvasScalerDifferent = IsCanvasScalerDifferent(designCanvasScaler, referenceCanvasScaler);
            bool isDifferent = isCanvasDifferent || isCanvasScalerDifferent;

            if (isDifferent)
            {
                string message = "设置不一致：\n";
                if (isCanvasDifferent) message += "• Canvas 设置不同\n";
                if (isCanvasScalerDifferent) message += "• CanvasScaler 设置不同\n";
                message += "\n这可能导致测试时的显示效果与实际运行时不同。";

                EditorGUILayout.HelpBox(message, MessageType. Warning);

                if (GUILayout.Button("同步所有设置（从 UISettings. UIRoot 到 DesignUIRoot）", GUILayout.Height(30)))
                {
                    if (isCanvasDifferent)
                    {
                        SyncCanvasSettings(referenceCanvas, designCanvas);
                    }
                    if (isCanvasScalerDifferent)
                    {
                        SyncCanvasScalerSettings(referenceCanvasScaler, designCanvasScaler);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Canvas 和 CanvasScaler 设置已同步 ✓", MessageType.Info);
            }
        }

        /// <summary>
        /// 比较两个 Canvas 的设置是否不同
        /// </summary>
        private bool IsCanvasDifferent(Canvas canvas1, Canvas canvas2)
        {
            if (canvas1.renderMode != canvas2.renderMode) return true;
            if (canvas1.sortingOrder != canvas2.sortingOrder) return true;
            if (canvas1.pixelPerfect != canvas2.pixelPerfect) return true;
            if (canvas1.sortingLayerID != canvas2.sortingLayerID) return true;
            if (canvas1.targetDisplay != canvas2.targetDisplay) return true;
            if (canvas1.additionalShaderChannels != canvas2.additionalShaderChannels) return true;

            // 如果是 ScreenSpaceCamera 模式，检查 planeDistance
            if (canvas1.renderMode == RenderMode.ScreenSpaceCamera)
            {
                if (Mathf.Abs(canvas1.planeDistance - canvas2.planeDistance) > 0.001f) return true;
            }

            return false;
        }

        /// <summary>
        /// 比较两个 CanvasScaler 的设置是否不同
        /// </summary>
        private bool IsCanvasScalerDifferent(CanvasScaler scaler1, CanvasScaler scaler2)
        {
            if (scaler1.uiScaleMode != scaler2.uiScaleMode) return true;

            // 根据不同的缩放模式检查相应的属性
            switch (scaler1.uiScaleMode)
            {
                case CanvasScaler.ScaleMode. ConstantPixelSize:
                    if (Mathf.Abs(scaler1.scaleFactor - scaler2.scaleFactor) > 0.001f) return true;
                    break;

                case CanvasScaler. ScaleMode.ScaleWithScreenSize:
                    if (scaler1.referenceResolution != scaler2.referenceResolution) return true;
                    if (scaler1.screenMatchMode != scaler2.screenMatchMode) return true;
                    if (scaler1.screenMatchMode == CanvasScaler.ScreenMatchMode. MatchWidthOrHeight)
                    {
                        if (Mathf.Abs(scaler1.matchWidthOrHeight - scaler2.matchWidthOrHeight) > 0.001f) return true;
                    }
                    break;

                case CanvasScaler.ScaleMode. ConstantPhysicalSize:
                    if (scaler1.physicalUnit != scaler2.physicalUnit) return true;
                    if (Mathf. Abs(scaler1.fallbackScreenDPI - scaler2.fallbackScreenDPI) > 0.001f) return true;
                    if (Mathf.Abs(scaler1.defaultSpriteDPI - scaler2.defaultSpriteDPI) > 0.001f) return true;
                    break;
            }

            if (Mathf.Abs(scaler1.referencePixelsPerUnit - scaler2.referencePixelsPerUnit) > 0.001f) return true;
            if (Mathf.Abs(scaler1.dynamicPixelsPerUnit - scaler2.dynamicPixelsPerUnit) > 0.001f) return true;

            return false;
        }

        /// <summary>
        /// 同步 Canvas 设置：从 source 到 target
        /// </summary>
        private void SyncCanvasSettings(Canvas source, Canvas target)
        {
            Undo.RecordObject(target, "Sync Canvas Settings");

            target.renderMode = source.renderMode;
            target.sortingOrder = source.sortingOrder;
            target.pixelPerfect = source.pixelPerfect;
            target.sortingLayerID = source.sortingLayerID;
            target.targetDisplay = source.targetDisplay;
            target.additionalShaderChannels = source.additionalShaderChannels;

            if (source.renderMode == RenderMode. ScreenSpaceCamera)
            {
                target.planeDistance = source.planeDistance;
            }

            EditorUtility.SetDirty(target);
            
            Debug.Log($"Canvas 设置已从 {source.gameObject.name} 同步到 {target. gameObject.name}");
        }

        /// <summary>
        /// 同步 CanvasScaler 设置：从 source 到 target
        /// </summary>
        private void SyncCanvasScalerSettings(CanvasScaler source, CanvasScaler target)
        {
            Undo. RecordObject(target, "Sync CanvasScaler Settings");

            target.uiScaleMode = source.uiScaleMode;
            target.referencePixelsPerUnit = source.referencePixelsPerUnit;
            target.dynamicPixelsPerUnit = source. dynamicPixelsPerUnit;

            // 根据缩放模式同步相应的属性
            switch (source.uiScaleMode)
            {
                case CanvasScaler.ScaleMode. ConstantPixelSize:
                    target.scaleFactor = source.scaleFactor;
                    break;

                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    target.referenceResolution = source. referenceResolution;
                    target.screenMatchMode = source. screenMatchMode;
                    target.matchWidthOrHeight = source.matchWidthOrHeight;
                    break;

                case CanvasScaler.ScaleMode.ConstantPhysicalSize:
                    target.physicalUnit = source.physicalUnit;
                    target.fallbackScreenDPI = source.fallbackScreenDPI;
                    target.defaultSpriteDPI = source.defaultSpriteDPI;
                    break;
            }

            EditorUtility.SetDirty(target);
            
            Debug.Log($"CanvasScaler 设置已从 {source.gameObject.name} 同步到 {target.gameObject.name}");
        }
    }
}

#endif