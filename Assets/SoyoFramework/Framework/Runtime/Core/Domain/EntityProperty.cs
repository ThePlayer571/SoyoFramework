namespace SoyoFramework.Framework.Runtime.Core.Domain
{
    public abstract class EntityProperty<TEntity>
    {
        protected TEntity Self { get; }

        protected EntityProperty(TEntity self)
        {
            Self = self;
        }
    }
}