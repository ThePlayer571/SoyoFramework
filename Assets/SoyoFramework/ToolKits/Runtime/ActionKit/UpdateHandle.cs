using System.Threading;

namespace SoyoFramework.ToolKits.Runtime.ActionKit
{
    public struct UpdateHandle
    {
        private CancellationTokenSource _cts;
        private bool _isCanceled;

        internal UpdateHandle(CancellationTokenSource cts)
        {
            _cts = cts;
            _isCanceled = false;
        }

        /// <summary>
        /// 停止每帧任务
        /// </summary>
        public void Cancel()
        {
            if (_isCanceled) return;
            _isCanceled = true;
            _cts?.Cancel();
            _cts = null;
        }
    }
}