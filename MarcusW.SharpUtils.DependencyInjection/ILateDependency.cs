namespace MarcusW.SharpUtils.DependencyInjection
{
    public interface ILateDependency<T>
    {
        bool IsValueCreated { get; }

        T Value { get; }
    }
}
