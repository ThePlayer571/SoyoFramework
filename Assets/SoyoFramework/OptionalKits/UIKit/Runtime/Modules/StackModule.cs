using System.Collections.Generic;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using SoyoFramework.OptionalKits.UIKit.Runtime.Pages;
using Unity.Plastic.Antlr3.Runtime.Misc;
using Action = System.Action;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Modules
{
    public class StackModule : UIModule
    {
        public StackModule(IUIViewHost host) : base(host)
        {
        }

        private readonly ListStack<IStackItem> _stack = new();

        public bool TryPush(IStackItem item)
        {
            if (item == null || _stack.Contains(item))
            {
                return false;
            }

            _stack.Add(item);
            item.OnPushed();
            return true;
        }

        public IStackItem Pop()
        {
            var item = _stack.Pop();
            item.OnPopped();
            return item;
        }

        public IStackItem Peek()
        {
            return _stack.Peek();
        }

        public bool Remove(IStackItem item)
        {
            return _stack.Remove(item);
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }

    public interface IStackItem
    {
        void OnPopped();
        void OnPushed();
    }

    public class StackItem : IStackItem
    {
        private Action _onPopped;
        private Action _onPushed;

        public StackItem WithOnPopped(Action onPopped)
        {
            _onPopped = onPopped;
            return this;
        }

        public StackItem WithOnPushed(Action onPushed)
        {
            _onPushed = onPushed;
            return this;
        }

        void IStackItem.OnPopped()
        {
            _onPopped?.Invoke();
        }

        void IStackItem.OnPushed()
        {
            _onPushed?.Invoke();
        }
    }
}