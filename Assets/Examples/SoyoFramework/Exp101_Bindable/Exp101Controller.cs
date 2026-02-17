using System;
using SoyoFramework.Framework.Runtime.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.SoyoFramework.Exp101_Bindable
{
    [ExecuteAlways]
    public class Exp101Controller : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private Text BindablePropertyText;
        [SerializeField] private Text EasyEventText;

        [Header("BindableProperty")] [SerializeField]
        private BindableProperty<string> StringBindableProperty;

        [SerializeField] private BindableProperty<DataA> DataABindableProperty;
        [SerializeField] private BindableProperty<DataB> DataBBindableProperty;
        [SerializeField] private BindableProperty<DataC> DataCBindableProperty;
        [SerializeField] private BindableProperty<DataD> DataDBindableProperty;
        [SerializeField] private BindableProperty<DataE> DataEBindableProperty;
        [SerializeField] private BindableProperty<BindableProperty<string>> NestedBindableProperty;

        [Header("EasyEvent")] [SerializeField] private EasyEvent NoneEasyEvent;
        [SerializeField] private EasyEvent<string> StringEasyEvent;
        [SerializeField] private EasyEvent<string, string> TwoStringEasyEvent;
        [SerializeField] private EasyEvent<string, string, string> ThreeStringEasyEvent;
        [SerializeField] private EasyEvent<DataA> DataAEasyEvent;
        [SerializeField] private EasyEvent<DataA, string> DataAAndStringEasyEvent;
        [SerializeField] private EasyEvent<DataA, DataA, DataA> ThreeDataAEasyEvent;
        [SerializeField] private EasyEvent<DataC> DataBEasyEvent;
        [SerializeField] private EasyEvent<DataD> DataDEasyEvent;
        [SerializeField] private EasyEvent<DataE> DataEEasyEvent;

        [Serializable]
        public class DataA
        {
            public string str_1;
            public string str_2;

            public override string ToString()
            {
                return $"str_1={str_1}, str_2={str_2}";
            }
        }

        [Serializable]
        public class DataB
        {
            public BindableProperty<string> bindableStr_1;

            public override string ToString()
            {
                return $"bindableStr_1={bindableStr_1.Value}";
            }
        }

        [Serializable]
        public class DataC
        {
            public DataA data_1;
            public DataA data_2;
            public DataB data_3;

            public override string ToString()
            {
                return $"data_1=[{data_1}], data_2=[{data_2}], data_3=[{data_3}]";
            }
        }

        public class DataD
        {
            public string str_1;
            public string str_2;
            public int int_1;
        }

        [Serializable]
        public class DataE
        {
            public string str_1;

            override public string ToString()
            {
                return $"str_1={str_1}";
            }
        }

        private void OnEnable()
        {
            StringBindableProperty.Register(WriteToBindablePropertyText);
            DataABindableProperty.Register(WriteToBindablePropertyText);
            DataBBindableProperty.Register(WriteToBindablePropertyText);
            DataCBindableProperty.Register(WriteToBindablePropertyText);
            DataDBindableProperty.Register(WriteToBindablePropertyText);
            DataEBindableProperty.Register(WriteToBindablePropertyText);
            NestedBindableProperty.Register(WriteToBindablePropertyText);
            //
            NoneEasyEvent.Register(() => EasyEventText.text = "NoneEasyEvent 被触发了");
            StringEasyEvent.Register(str => EasyEventText.text = $"StringEasyEvent 被触发了，参数：{str}");
            TwoStringEasyEvent.Register((str1, str2) =>
                EasyEventText.text = $"TwoStringEasyEvent 被触发了，参数：{str1}，{str2}");
            ThreeStringEasyEvent.Register((str1, str2, str3) =>
                EasyEventText.text = $"ThreeStringEasyEvent 被触发了，参数：{str1}，{str2}，{str3}");
            DataAEasyEvent.Register(data => EasyEventText.text = $"DataAEasyEvent 被触发了，参数：{data}");
            DataAAndStringEasyEvent.Register((data1, data2) =>
                EasyEventText.text = $"TwoDataAEasyEvent 被触发了，参数：{data1}，{data2}");
            ThreeDataAEasyEvent.Register((data1, data2, data3) =>
                EasyEventText.text = $"ThreeDataAEasyEvent 被触发了，参数：{data1}，{data2}，{data3}");
            DataEEasyEvent.Register(data => EasyEventText.text = $"DataEEasyEvent 被触发了，参数：{data}");
            DataBEasyEvent.Register(data => EasyEventText.text = $"DataBEasyEvent 被触发了，参数：{data}");
            DataDEasyEvent.Register(data => EasyEventText.text = $"DataDEasyEvent 被触发了，参数：{data}");
        }

        private void OnDisable()
        {
            StringBindableProperty.UnRegister(WriteToBindablePropertyText);
            DataABindableProperty.UnRegister(WriteToBindablePropertyText);
            DataBBindableProperty.UnRegister(WriteToBindablePropertyText);
            DataCBindableProperty.UnRegister(WriteToBindablePropertyText);
            DataDBindableProperty.UnRegister(WriteToBindablePropertyText);
            NestedBindableProperty.UnRegister(WriteToBindablePropertyText);
        }

        private void WriteToBindablePropertyText(object obj)
        {
            BindablePropertyText.text = $"BindableProperty 当前值：\n{obj}";
        }
    }
}