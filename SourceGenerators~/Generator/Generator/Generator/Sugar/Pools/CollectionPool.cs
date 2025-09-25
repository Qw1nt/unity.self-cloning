using System.Collections.Concurrent;
using System.Collections.Generic;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Interfaces;

namespace Generator.Sugar.Pools;

public static class CollectionPool
{
    private const int MaxSize = 32;
    
    private static readonly ConcurrentStack<Queue<ISemanticStructBuilder>> QueueSemanticStruct = new();
    private static readonly ConcurrentStack<List<ISemanticStructBuilder>> ListSemanticStruct = new();
    private static readonly ConcurrentStack<List<AttributeInfo>> Attributes = new();
    private static readonly ConcurrentStack<HashSet<string>> StringHashSet = new();

    public static Queue<ISemanticStructBuilder> GetQueueSemanticStructBuilder()
    {
        return QueueSemanticStruct.TryPop(out var result) ? result : new Queue<ISemanticStructBuilder>();
    }

    public static List<ISemanticStructBuilder> GetListSemanticStructBuilder()
    {
        return ListSemanticStruct.TryPop(out var result) ? result : new List<ISemanticStructBuilder>();
    }

    public static List<AttributeInfo> GetAttributes()
    {
        return Attributes.TryPop(out var result) ? result : new List<AttributeInfo>();
    }

    public static HashSet<string> GetStringHashSet()
    {
        return StringHashSet.TryPop(out var result) ? result : new HashSet<string>();
    }

    public static void Return(Queue<ISemanticStructBuilder> value)
    {
        if(QueueSemanticStruct.Count >= MaxSize)
            return;

        value.Clear();
        QueueSemanticStruct.Push(value);
    }
    
    public static void Return(List<ISemanticStructBuilder> value)
    {
        if(ListSemanticStruct.Count >= MaxSize)
            return;

        value.Clear();
        ListSemanticStruct.Push(value);
    }
    
    public static void Return(List<AttributeInfo> value)
    {
        if(Attributes.Count >= MaxSize)
            return;

        value.Clear();
        Attributes.Push(value);
    }    
    
    public static void Return(HashSet<string> value)
    {
        if(StringHashSet.Count >= MaxSize)
            return;

        value.Clear();
        StringHashSet.Push(value);
    }
}