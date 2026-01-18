using System.Collections.Generic;
using System.Linq;
using SoyoFramework.Framework.Runtime.Utils.FluentAPI;
using Action = System.Action;

namespace SoyoFramework.OptionalKits.UIKit.Runtime.Page.Modules
{
    public class StackModule : UIModule
    {
        public StackModule(IUIViewHost host) : base(host)
        {
        }

        private readonly List<IStackItem> _stack = new();

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
            return _stack.Last();
        }

        public bool Remove(IStackItem item)
        {
            return _stack.Remove(item);
        }

        public void PopAll()
        {
            while (_stack.Count > 0)
            {
                Pop();
            }
        }
        
        public int Count => _stack.Count;
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