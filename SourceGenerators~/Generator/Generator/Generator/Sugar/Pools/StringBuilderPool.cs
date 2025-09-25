using System.Collections.Concurrent;
using System.Text;

namespace SourceGenerators.Sugar.Pools;

public static class StringBuilderPool
{
    private const int PoolSize = 32;
    
    private static readonly ConcurrentStack<StringBuilder> Pool = new();

    public static StringBuilder Get()
    {
        return Pool.TryPop(out var builder) ? builder : new StringBuilder();
    }

    public static void Return(StringBuilder builder)
    {
        if(Pool.Count >= PoolSize)
            return;

        builder.Clear();
        Pool.Push(builder);
    }

    public static string ToStringAndReturn(this StringBuilder builder)
    {
        var result = builder.ToString();
        Return(builder);
        
        return result;
    }
}