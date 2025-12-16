namespace SoyoFramework.Framework.Runtime.Core.CommandProfiler
{
    internal sealed class CommandStat
    {
        /// <summary>累计发送次数</summary>
        internal int TotalCount;

        /// <summary>当前时间桶（1 秒）内的发送次数</summary>
        internal int CurrentSecondCount;

        /// <summary>历史单秒内的最大峰值</summary>
        internal int PeakPerSecond;
    }
}