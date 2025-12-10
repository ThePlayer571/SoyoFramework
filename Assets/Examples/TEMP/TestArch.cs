using Cysharp.Threading.Tasks;
using SoyoFramework.Framework.Runtime.Core;
using SoyoFramework.Framework.Runtime.Core.Modules;
using SoyoFramework.Framework.Runtime.LogKit;
using SoyoFramework.Framework.Runtime.UsefulTools;
using UnityEngine;

namespace Examples.TEMP
{
    public class TestArch : AbstractArchitecture
    {
        [RuntimeInitializeOnLoadMethod]
        private static void InitAll()
        {
            "call init".LogInfo();
            var arch = new TestArch();
            arch.Init();
        }

        protected override void OnInit()
        {
            RegisterModel<TestModel>(new TestModel());
            RegisterSystem<TestSystem>(new TestSystem());
        }

        protected override void OnDeinit()
        {
        }
    }

    public class TestModel : AbstractModel
    {
        public int a = 1;

        protected override void OnPreInit()
        {
            a = 114;
        }

        protected override void OnDeinit()
        {
            "Model 销毁".LogInfo();
        }
    }

    public class TestSystem : AbstractSystem
    {
        private IProxy<TestModel> _testModel;

        protected override void OnInit()
        {
            _testModel = this.GetModel<TestModel>();
            $"a is {_testModel.Get.a}".LogInfo();

            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(1f);
                "一秒后，销毁TestModel".LogInfo();
                this.UnRegisterModel<TestModel>();
                $"TestModel is null: {_testModel.Get == null}".LogInfo();
            });
        }
    }
}