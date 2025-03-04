if (args.Length != 1)
{
    Console.WriteLine("Usage: MyNUnit <path-to-tests>");
    return;
}

var path = args[0];
var runner = new MyNUnit.MyNUnit();

if (!Directory.Exists(path))
{
    Console.WriteLine("Directory doesn't exist: " + path);
    return;
}

var result = await runner.ExecuteTestsAsync(path);
MyNUnit.MyNUnit.DisplayTestResults(result);