namespace SoyoFramework.Runtime.Domain
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