/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Interfaces;
using SourceGenerators.Sugar.Pools;
using SourceGenerators.Sugar.SemanticBuilding;

namespace SourceGenerators.Sugar.Base;

public abstract class IncrementalSourceGeneratorBase
{
    private readonly HashSet<string> _usings = new();
    private readonly List<AttributeInfo> _attributesStore = new();

    protected readonly StringBuilder Builder;

    private NamespaceBuilder _root = null!;
    private int _reservedSpaces;

    private ISemanticStructBuilder? _structBuilderContext;

    public IncrementalSourceGeneratorBase()
    {
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

    protected void Using(string usingLine)
    {
        _usings.Add(usingLine);
    }

    protected void Using(ICollection<string> usings)
    {
        foreach (var @using in usings)
            Using(@using);
    }

    protected void Attribute(AttributeInfo source)
    {
        _attributesStore.Add(source);
    }

    protected void Public()
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

    protected void Space()
    {
        _reservedSpaces++;
    }

    protected void Namespace(ITypeSymbol type, Action<NamespaceBuilder> context)
    {
        Namespace(type.ContainingNamespace.ToDisplayString(), context);
    }

    protected void Namespace(INamespaceSymbol namespaceSymbol, Action<NamespaceBuilder> context)
    {
        Namespace(namespaceSymbol.ToDisplayString(), context);
    }

    protected void Namespace(string namespaceName, Action<NamespaceBuilder> context)
    {
        _root = SemanticBuilderPool.GetNamespace();
        _root.Namespace = namespaceName;
        _root.ContextId = Guid.NewGuid();

        StructBuilderContext = _root;
        var currentContext = StructBuilderContext;
        context.Invoke(_root);

        StructBuilderContext = currentContext;
    }

    protected void Class(string modifier, string name, Action<LogicContainer> context)
    {
        Container(modifier, name, MethodContainerType.Class, context);
    }

    protected void Struct(string modifier, string name, Action<LogicContainer> context)
    {
        Container(modifier, name, MethodContainerType.Struct, context);
    }

    protected void Container(string modifier, string name, MethodContainerType type, Action<LogicContainer> context)
    {
        var classBuilder = SemanticBuilderPool.GetContainer();
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

    protected void Const(Accessibility accessibility, string type, string name, string value)
    {
        CreateDataField(accessibility.AsString(),
            type,
            name,
            null!,
            FieldBuilderType.Const,
            value: value
        );
    }

    protected void Field(Accessibility accessibility, string type, string name, Action<FieldBuilder>? builder = null)
    {
        CreateDataField(accessibility.AsString(), type, name, builder!, FieldBuilderType.Field);
    }

    protected void Field(string modifier, string type, string name, Func<FieldBuilder, FieldBuilder>? builder = null)
    {
        CreateDataField(modifier, type, name, null!, FieldBuilderType.Field);
    }

    protected void Property(Accessibility accessibility, string type, string name, Action<FieldBuilder> builder)
    {
        CreateDataField(accessibility.AsString(), type, name, builder, FieldBuilderType.Property);
    }

    protected void Property(string accessibility, string type, string name, Action<FieldBuilder> builder)
    {
        CreateDataField(accessibility, type, name, builder, FieldBuilderType.Property);
    }

    private void CreateDataField(string modifier, string type, string name, Action<FieldBuilder> builder,
        FieldBuilderType builderType, string? value = null)
    {
        var field = new FieldBuilder
        {
            Modifier = modifier,
            ValueType = type,
            Type = builderType,
            Name = name,
            Value = value,
            ContextId = Guid.NewGuid()
        };

        var context = StructBuilderContext;

        StructBuilderContext!.AddChild(field);
        StructBuilderContext = field;

        if (builder != null!)
            builder.Invoke(field);

        StructBuilderContext = context;
    }

    protected void Constructor(string modifier, ITypeSymbol type, Action<MethodStructBuilder> builder)
    {
        Method(modifier, string.Empty, type.Name, builder);
    }

    protected void Constructor(string modifier, ITypeSymbol type, string args, Action<MethodStructBuilder> builder)
    {
        Method(modifier, string.Empty, type.Name, args, builder);
    }

    protected void Method(string modifier, string type, string name, Action<MethodStructBuilder> builder)
    {
        Method(modifier, type, name, string.Empty, builder);
    }

    protected void Method(string modifier, string type, string name, string args, Action<MethodStructBuilder> builder)
    {
        var methodBuilder = SemanticBuilderPool.GetMethod();
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
        var queue = CollectionPool.GetQueueSemanticStructBuilder();
        queue.Enqueue(_root);

        var attributes = CollectionPool.GetAttributes();

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
        CollectionPool.Return(queue);
        CollectionPool.Return(attributes);
        
        return count;
    }

    protected void Build()
    {
        var indent = 0;

        _root.UsingsCount = BuildUsings();
        _root.Build(new SemanticBuildingContext(Builder), ref indent);
    }

    protected void AddSource(SourceProductionContext context, string fileName)
    {
        Build();
        context.AddSource($"{fileName}.g.cs", SourceText.From(Builder.ToString(), Encoding.UTF8));
    }

    public abstract void Initialize(IncrementalGeneratorInitializationContext context);
}*/