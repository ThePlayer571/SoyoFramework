using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.OptionalKits.ProcedureKit.Runtime.DataClasses
{
    public class ProcedureChangeInfo
    {
        public ProcedureChangeParas Paras;

        public class ProcedureChangeParas
        {
            private List<(string, object)> _paras;

            public T GetPara<T>(string key)
            {
                var found = TryGetParaValue<T>(key, out var result);

                if (!found)
                {
                    $"未找到对应的PhaseChangePara，Key: {key}, TargetType: {typeof(T).FullName}".LogError();
                }

                return result;
            }

            public T GetPara<T>(string key, T defaultValue)
            {
                var found = TryGetParaValue<T>(key, out var result);
                return found ? result : defaultValue;
            }

            private bool TryGetParaValue<T>(string key, out T result)
            {
                result = default;

                // 查找参数
                foreach (var para in _paras)
                {
                    if (para.Item1 == key)
                    {
                        // 类型转换
                        try
                        {
                            if (para.Item2 is T typedValue)
                            {
                                result = typedValue;
                                return true;
                            }

                            $"无法将参数转换为目标类型。Key:  {key}, TargetType: {typeof(T).FullName}, ActualType: {para.Item2.GetType().FullName}"
                                .LogError();
                            return false;
                        }
                        catch (Exception e)
                        {
                            $"PhaseChangePara类型转换失败，Key: {key}, TargetType: {typeof(T).FullName}, Exception: {e}"
                                .LogError();
                            return false;
                        }
                    }
                }

                return false;
            }

            public ProcedureChangeParas(IEnumerable<(string, object)> paras)
            {
                _paras = paras.ToList();
            }

            public ProcedureChangeParas(params (string, object)[] paras)
            {
                _paras = paras.ToList();
            }
        }
    }
}