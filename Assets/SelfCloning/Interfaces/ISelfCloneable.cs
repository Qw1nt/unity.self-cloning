namespace SelfCloning.Interfaces
{
    public interface ISelfCloneable
    {
        object MakeObjectClone();
    }

    public interface ISelfCloneable<out T> : ISelfCloneable
        where T : ISelfCloneable
    {
        T MakeTypedClone();
    }
}