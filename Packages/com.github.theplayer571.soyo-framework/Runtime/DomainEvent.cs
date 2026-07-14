using SoyoFramework.Utils;

namespace SoyoFramework
{
    public class DomainEvent : EasyEvent
    {
    }

    public class DomainEvent<T> : EasyEvent<T>
    {
    }

    public class DomainEvent<T1, T2> : EasyEvent<T1, T2>
    {
    }

    public class DomainEvent<T1, T2, T3> : EasyEvent<T1, T2, T3>
    {
    }
}