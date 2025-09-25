using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Generator.Common;
using Generator.Sugar.Pools;
using Microsoft.CodeAnalysis;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Interfaces;
using SourceGenerators.Sugar.Pools;
using SourceGenerators.Sugar.SemanticBuilding;

namespace SourceGenerators.Sugar.Common;

public class SugarGenerator<T>
{
    private readonly HashSet<string> _usings;
    private readonly List<AttributeInfo> _attributesStore;

    public StringBuilder Builder;

    private NamespaceBuilder _root = null!;
    private int _reservedSpaces;

    private ISemanticStructBuilder? _structBuilderContext;

    public T Context;

    public SugarGenerator()
    {
        _usings = new HashSet<string>();
        _attributesStore = new List<AttributeInfo>();
        Builder = StringBuilderPool.Get();
    }

    private ISemanticStructBuilder? StructBuilderContext
    {
        get => _structBuilderContext;
        set
        {
            _structBuilderContext = value;

            TryAddAttributesToContext();
            StructBuilderContext?.AddSpace(_reservedSpaces);

            _reservedSpaces = 0;
        }
    }

    public void Using(string usingLine)
    {
        _usings.Add(usingLine);
    }

    public void Using(ICollection<string> usings)
    {
        foreach (var @using in usings)
            Using(@using);
    }

    public void Attribute(AttributeInfo source)
    {
        _attributesStore.Add(source);
    }    
    
    public void AddAggressiveMethodImplAttribute()
    {
        _attributesStore.Add(AttributeInfo.MethodImpl(MethodImplOptions.AggressiveInlining));
    }    
    
    public void AddIl2CppAttributes()
    {
        _attributesStore.Add(AttributeInfo.Il2CppAttribute(Il2CppOption.NullChecks));
        _attributesStore.Add(AttributeInfo.Il2CppAttribute(Il2CppOption.ArrayBoundsChecks));
        _attributesStore.Add(AttributeInfo.Il2CppAttribute(Il2CppOption.DivideByZeroChecks));
    }

    public void Public()
    {
    }

    private void TryAddAttributesToContext()
    {
        if (StructBuilderContext == null || _attributesStore.Count == 0)
            return;

        foreach (var builderAttribute in _attributesStore)
            StructBuilderContext.AddAttribute(builderAttribute);

        _attributesStore.Clear();
    }

    public void Space()
    {
        _reservedSpaces++;
    }

    public void Namespace(ITypeSymbol type, Action<NamespaceBuilder> context)
    {
        Namespace(type.ContainingNamespace.ToDisplayString(), context);
    }

    public void Namespace(INamespaceSymbol namespaceSymbol, Action<NamespaceBuilder> context)
    {
        Namespace(namespaceSymbol.ToDisplayString(), context);
    }

    public void Namespace(string namespaceName, Action<NamespaceBuilder> context)
    {
        _root = new NamespaceBuilder();
        _root.Namespace = namespaceName;
        _root.ContextId = Guid.NewGuid();

        StructBuilderContext = _root;
        var currentContext = StructBuilderContext;
        context.Invoke(_root);

        StructBuilderContext = currentContext;
    }

    public void Class(string modifier, string name, Action<LogicContainer> context)
    {
        Container(modifier, name, MethodContainerType.Class, context);
    }

    public void Struct(string modifier, string name, Action<LogicContainer> context)
    {
        Container(modifier, name, MethodContainerType.Struct, context);
    }

    public void Container(string modifier, string name, MethodContainerType type, Action<LogicContainer> context)
    {
        var classBuilder = SemanticBuilderPool.GetContainer(); //new LogicContainer();
        classBuilder.Modifier = modifier;
        classBuilder.Name = name;
        classBuilder.Type = type;
        classBuilder.ContextId = Guid.NewGuid();

        var currentContext = StructBuilderContext;
        StructBuilderContext!.AddChild(classBuilder);
        StructBuilderContext = classBuilder;

        context.Invoke(classBuilder);

        StructBuilderContext = currentContext;
    }

    public void Const(Accessibility accessibility, string type, string name, string value)
    {
        CreateDataField(accessibility.AsString(),
            type,
            name,
            null!,
            FieldBuilderType.Const,
            value: value
        );
    }

    public void Field(Accessibility accessibility, string type, string name, Action<FieldBuilder>? builder = null)
    {
        CreateDataField(accessibility.AsString(), type, name, builder!, FieldBuilderType.Field);
    }

    public void Field(string modifier, string type, string name, Func<FieldBuilder, FieldBuilder>? builder = null)
    {
        CreateDataField(modifier, type, name, null!, FieldBuilderType.Field);
    }

    public void Property(Accessibility accessibility, string type, string name, Action<FieldBuilder> builder)
    {
        CreateDataField(accessibility.AsString(), type, name, builder, FieldBuilderType.Property);
    }

    public void Property(string accessibility, string type, string name, Action<FieldBuilder> builder)
    {
        CreateDataField(accessibility, type, name, builder, FieldBuilderType.Property);
    }

    private void CreateDataField(string modifier, string type, string name, Action<FieldBuilder> builder,
        FieldBuilderType builderType, string? value = null)
    {
        var field = SemanticBuilderPool.GetFieldBuilder();

        field.Modifier = modifier;
        field.ValueType = type;
        field.Type = builderType;
        field.Name = name;
        field.Value = value;
        field.ContextId = Guid.NewGuid();

        var context = StructBuilderContext;

        StructBuilderContext!.AddChild(field);
        StructBuilderContext = field;

        if (builder != null!)
            builder.Invoke(field);

        StructBuilderContext = context;
    }

    public void Constructor(string modifier, ITypeSymbol type, Action<MethodStructBuilder> builder)
    {
        Method(modifier, string.Empty, type.Name, builder);
    }

    public void Constructor(string modifier, ITypeSymbol type, string args, Action<MethodStructBuilder> builder)
    {
        Method(modifier, string.Empty, type.Name, args, builder);
    }

    public void Method(string modifier, string type, string name, Action<MethodStructBuilder> builder)
    {
        Method(modifier, type, name, string.Empty, builder);
    }
    
    public void Method(string modifier, Type type, string name, Action<MethodStructBuilder> builder)
    {
        Method(modifier, type.Name, name, string.Empty, builder);
    }

    public void Method(string modifier, string type, string name, string args, Action<MethodStructBuilder>? builder)
    {
        var methodBuilder = SemanticBuilderPool.GetMethod(); //new MethodStructBuilder();

        methodBuilder.Modifier = modifier;
        methodBuilder.Type = type;
        methodBuilder.Name = name;
        methodBuilder.Args = args;
        methodBuilder.ContextId = Guid.NewGuid();

        var context = StructBuilderContext;
        StructBuilderContext!.AddChild(methodBuilder);
        StructBuilderContext = methodBuilder;

        builder?.Invoke(methodBuilder);

        StructBuilderContext = context;
    }

    private int BuildUsings()
    {
        var queue = CollectionPool.GetQueueSemanticStructBuilder(); //new Queue<ISemanticStructBuilder>();// CollectionPool.GetQueueSemanticStructBuilder();
        queue.Enqueue(_root);

        var attributes = CollectionPool.GetAttributes(); //new List<AttributeInfo>(); //CollectionPool.GetAttributes();

        try
        {
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                attributes.AddRange(item.GetAttributes());

                var children = item.GetChild();

                if (children == null)
                    continue;

                foreach (var child in children)
                    queue.Enqueue(child);
            }

            var usings = attributes.Select(x => x.Namespace);

            foreach (var usingLine in usings)
                _usings.Add(usingLine);

            var count = _usings.Count;

            foreach (var usingLine in _usings)
            {
                Builder.Append("using ");
                Builder.Append(usingLine);
                Builder.Append(';');
                Builder.Append('\n');
            }

            _usings.Clear();
            return count;
        }
        finally
        {
            CollectionPool.Return(queue);
            CollectionPool.Return(attributes);
        }
    }

    public string BuildContent()
    {
        var indent = 0;

        _root.UsingsCount = BuildUsings();
        _root.Build(new SemanticBuildingContext(Builder), ref indent);

        return Builder.ToStringAndReturn();
    }
}