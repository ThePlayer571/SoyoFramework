using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Framework.Runtime.LogKit;

namespace SoyoFramework.Framework.Runtime.ProcedureKit
{
    public class ProcedureChangeInfo
    {
        public ProcedureChangeParas Paras;

        public class ProcedureChangeParas
        {
            private List<(string, object)> _paras;


            public T GetPara<T>(string key)
            {
                try
                {
                    var para = _paras.FirstOrDefault(p => p.Item1 == key).Item2;
                    if (para == null)
                    {
                        $"PhaseChangePara获取失败，Key: {key}, TargetType: {typeof(T).FullName}, 未找到对应参数".LogError();
                        return default;
                    }

                    return (T)para;
                }
                catch (InvalidCastException e)
                {
                    $"PhaseChangePara类型转换失败，Key: {key}, TargetType: {typeof(T).FullName}, Exception: {e}".LogError();
                }
                catch (Exception e)
                {
                    $"PhaseChangePara获取失败，Key: {key}, TargetType: {typeof(T).FullName}, Exception: {e}".LogError();
                    return default;
                }

                return default;
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