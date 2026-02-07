using SoyoFramework.Framework.Runtime.Utils.LogKit;

namespace SoyoFramework.ToolKits.Runtime.RandomKit.RandomPool
{
    public interface IRandomPoolEntry<out TOutput>
        where TOutput : class
    {
        /// <summary>
        /// 价值
        /// </summary>
        float Value { get; }

        /// <summary>
        /// 权重。数值越高，越容易被选中。必须大于或等于0
        /// </summary>
        float Weight { get; }

        /// <summary>
        /// 返回值
        /// </summary>
        TOutput Output { get; }

        /// <summary>
        /// 最大被返回的次数，值小于等于0代表无限制
        /// </summary>
        int MaxReturnCount { get; }
    }

    public class CustomRandomPoolEntry<TOutput> : IRandomPoolEntry<TOutput>
        where TOutput : class
    {
        public float Value { get; }
        public float Weight { get; }
        public TOutput Output { get; }
        public int MaxReturnCount { get; }

        public CustomRandomPoolEntry(float value, float weight, TOutput output, int maxReturnCount)
        {
            Value = value;
            
            if (weight < 0)
            {
                $"参数weight不能小于0，已自动设为0".LogWarning();
                weight = 0;
            }

            Weight = weight;
            Output = output;
            MaxReturnCount = maxReturnCount;
        }

        public CustomRandomPoolEntry(float value, float weight, TOutput output) : this(value, weight, output, 0)
        {
            
        }
    }
}