namespace KR2.Tests;

public class ReflectorTests
{
    [Test]
    public void TestPrintStructure_CreatesFileWithCorrectStructure()
    {
        var reflector = new Reflector();
        var type = typeof(You<int>);
        var expectedFileName = $"{type.Name}.cs";

        reflector.PrintStructure(type);

        Assert.That(File.Exists(expectedFileName), Is.True);

        var fileContent = File.ReadAllText(expectedFileName);

        Assert.That(fileContent, Does.Contain($"public class {type.Name}"));

        Assert.That(fileContent, Does.Contain("private String <YouName>k__BackingField;"));
        Assert.That(fileContent, Does.Contain("private Int32 <Data>k__BackingField;"));

        Assert.That(fileContent, Does.Contain("public String ReturnName()"));
        Assert.That(fileContent, Does.Contain("private Int32 ReturnData()")); // Здесь мы изменили проверку

        File.Delete(expectedFileName);
    }

    [Test]
    public void TestDiffClasses_ReturnsCorrectDifferences()
    {
        var reflector = new Reflector();
        var firstClass = typeof(You<int>);
        var secondClass = typeof(Me<int>);

        var (diffFields, diffMethods) = reflector.DiffClasses(firstClass, secondClass);

        var diffFieldsList = diffFields.ToList();
        var diffMethodsList = diffMethods.ToList();

        Assert.That(diffFieldsList, Does.Contain("<YouName>k__BackingField"));
        Assert.That(diffFieldsList, Does.Contain("<Age>k__BackingField"));
    }
}

public class You<T>
{
    public string YouName { get; set; }

    private T Data { get; set; }

    public string ReturnName()
    {
        return YouName;
    }

    private T ReturnData()
    {
        return Data;
    }
}

public class Me<T>
{
    public int Age { get; set; }

    private T Data { get; set; }

    public int ReturnName()
    {
        return Age;
    }

    private T ReturnData()
    {
        return Data;
    }
}