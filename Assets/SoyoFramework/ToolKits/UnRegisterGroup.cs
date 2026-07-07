using System.Collections.Generic;
using SoyoFramework.Runtime.Utils.UnRegisters;

namespace SoyoFramework.ToolKits
{
    public class UnRegisterGroup : IUnRegister
    {
        private List<IUnRegister> _unRegisters = new();

        public void UnRegister()
        {
            foreach (var unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }

            _unRegisters.Clear();
        }

        public void Add(IUnRegister unRegister)
        {
            _unRegisters.Add(unRegister);
        }
    }
}