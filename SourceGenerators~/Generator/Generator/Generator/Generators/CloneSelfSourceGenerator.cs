using System;
using Microsoft.CodeAnalysis;
using SourceGenerators.Generators.Utils;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Pools;

namespace Generator.Generators;

public class CloneSelfSourceGenerator
{
    private static SugarGenerator<ClonedSelfInfo> SugarGenerator;
    
    public static void Generate(SourceProductionContext spc, in ClonedSelfInfo info)
    {
        try
        {
            var source = Generate(info);
            spc.AddSource($"{info.TypeName}.SelfClone.g.cs", source);
        }
        catch (Exception e)
        {
            Logger.LogException(nameof(CloneSelfSourceGenerator), nameof(Generate), e);
        }
    }

    public static string Generate(in ClonedSelfInfo info)
    {
        SugarGenerator = new()
        {
            Context = info
        };

        var bodyBuilder = StringBuilderPool.Get();
        bodyBuilder.Append("return new ");
        bodyBuilder.AppendLine(info.TypeName);
        bodyBuilder.AppendLine("{");
        
        foreach (var symbol in info.FieldsAndProperties)
        {
            if(symbol == null)
                continue;

            if (symbol.IsImplicitlyDeclared == true)
                continue;
            
            if(symbol is IPropertySymbol propertySymbol == true && propertySymbol.SetMethod == null)
                continue;
            
            if(symbol is IFieldSymbol fieldSymbol == true && fieldSymbol.IsReadOnly == true)
                continue;
            
            bodyBuilder.Append('\t');
            bodyBuilder.Append(symbol.Name);
            bodyBuilder.Append(" = ");
            bodyBuilder.Append(symbol.Name);
            bodyBuilder.Append(",\n");
        }

        bodyBuilder.Append("};");

        var createMethodsBody = bodyBuilder.ToStringAndReturn();
        
        SugarGenerator.Namespace(info.NamedTypeSymbol, _ =>
        {
            var modifier = StringBuilderPool.Get()
                .Append(SugarGenerator.Context.NamedTypeSymbol.AccessibilityAsString())
                .Append(" partial")
                .ToStringAndReturn();
            
            var name = StringBuilderPool.Get()
                .Append(SugarGenerator.Context.NamedTypeSymbol.Name)
                .Append(" : ")
                .Append(Constants.FullPath.ISelfCloneable)
                .Append('<')
                .Append(SugarGenerator.Context.NamedTypeSymbol.Name)
                .Append('>')
                .ToStringAndReturn();
            
            var containerType = SugarGenerator.Context.NamedTypeSymbol.TypeKind == TypeKind.Class
                ? MethodContainerType.Class
                : MethodContainerType.Struct;
            
            SugarGenerator.Container(modifier, name, containerType, _ =>
            {
                SugarGenerator.Method("public", "object", "MakeObjectClone", builder =>
                {
                    builder.Body = createMethodsBody;
                });         
                
                SugarGenerator.Space();
                
                SugarGenerator.Method("public", SugarGenerator.Context.TypeName, "MakeTypedClone", builder =>
                {
                    builder.Body = createMethodsBody;
                });
            });
        });

        return SugarGenerator.BuildContent();
    }
}