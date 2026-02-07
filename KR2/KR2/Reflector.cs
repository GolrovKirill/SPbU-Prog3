namespace KR2;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class Reflector
{
    /// <summary>
    /// Create file with some class.
    /// </summary>
    /// <param name="someClass">Need class.</param>
    public static void PrintStructure(Type someClass)
    {
        var className = someClass.Name;
        var fileName = $"{className}.cs";

        using var writer = new StreamWriter(fileName);
        writer.WriteLine($"public class {className}");
        writer.WriteLine("{");

        foreach (var field in someClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
        {
            writer.WriteLine($"    {GetView(field)} {GetType(field)} {field.Name};");
        }

        foreach (var method in someClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
        {
            writer.WriteLine($"    {GetView(method)} {GetReturnType(method)} {method.Name}({GetParameters(method)}){{ }}");
        }

        foreach (var depClass in someClass.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
        {
            writer.WriteLine($"    public class {depClass.Name}");
            writer.WriteLine("    {");
            PrintStructure(depClass);
            writer.WriteLine("    }");
        }

        writer.WriteLine("}");
    }

    /// <summary>
    /// Displays the differences between classes.
    /// </summary>
    /// <param name="firstClass">First class.</param>
    /// <param name="secondClass">Second class.</param>
    /// <returns>Differences.</returns>
    public static (IEnumerable<string>, IEnumerable<string>) DiffClasses(Type firstClass, Type secondClass)
    {
        var fieldsFirst = firstClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(f => f.Name).ToHashSet();
        var fieldsSecond = secondClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(f => f.Name).ToHashSet();

        var methodsA = firstClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(m => $"{m.ReturnType.Name} {m.Name}({GetParameters(m)})").ToHashSet();
        var methodsB = secondClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        .Select(m => $"{m.ReturnType.Name} {m.Name}({GetParameters(m)})").ToHashSet();

        var diffFields = fieldsFirst.Except(fieldsSecond).Union(fieldsSecond.Except(fieldsFirst));
        var diffMethods = methodsA.Except(methodsB).Union(methodsB.Except(methodsA));

        return (diffFields, diffMethods);
    }

    private static string GetView(FieldInfo field)
    {
        if (field.IsPublic)
        {
            return "public";
        }

        if (field.IsPrivate)
        {
            return "private";
        }

        if (field.IsFamily)
        {
            return "protected";
        }

        if (field.IsAssembly)
        {
            return "internal";
        }

        return string.Empty;
    }

    private static string GetType(FieldInfo field)
    {
        return field.FieldType.Name;
    }

    private static string GetView(MethodInfo method)
    {
        if (method.IsPublic)
        {
            return "public";
        }

        if (method.IsPrivate)
        {
            return "private";
        }

        if (method.IsFamily)
        {
            return "protected";
        }

        if (method.IsAssembly)
        {
            return "internal";
        }

        return string.Empty;
    }

    private static string GetReturnType(MethodInfo method)
    {
        return method.ReturnType.Name;
    }

    private static string GetParameters(MethodInfo method)
    {
        var parameters = method.GetParameters();
        return string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
    }
}