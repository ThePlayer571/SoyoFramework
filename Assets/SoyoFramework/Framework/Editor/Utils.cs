using System.Collections;
using System.Reflection;
using UnityEditor;

namespace SoyoFramework.Framework.Editor
{
    public static class Utils
    {
        public static object GetTargetObject(SerializedProperty property)
        {
            // 思路：根据路径不断拆解currentObj

            // 将propertyPath转换为可读性更良好的格式。约定：数组的string形如 someName[0]
            // bad: 如果非数组字段的名称为Array.data[x]的格式，会识别为数组元素处理。考虑到不会有这么奇怪的名称，没有处理这种情况
            var splitPath = property.propertyPath.Replace(".Array.data[", "[").Split(".");

            object currentObj = property.serializedObject.targetObject;
            foreach (var element in splitPath)
            {
                var isArray = element.Contains("[");
                if (isArray)
                {
                    var fieldName = element.Substring(0, element.IndexOf("["));
                    var index = int.Parse(element.Substring(element.IndexOf("[") + 1).TrimEnd(']'));

                    currentObj = GetFieldValue(currentObj, fieldName);

                    if (currentObj is IList list && index < list.Count)
                    {
                        currentObj = list[index];
                    }
                    else
                    {
                        // 获取失败：数组越界或类型不匹配
                        return null;
                    }
                }
                else
                {
                    currentObj = GetFieldValue(currentObj, element);
                }
            }

            return currentObj;
        }

        private static object GetFieldValue(object target, string fieldName)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            if (target == null) return null;

            var type = target.GetType();

            while (type != null)
            {
                var field = type.GetField(fieldName, flags);
                if (field != null) return field.GetValue(target);

                type = type.BaseType;
            }

            return null;
        }
    }
}