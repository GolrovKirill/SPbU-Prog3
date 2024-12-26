using MyNUnit;

if (args.Length != 1)
{
    Console.WriteLine("Incorrect input. Enter the path to the directory.");
    return 1;
}

var path = args[0];

var runner = new TestRunner();
runner.RunTest(path);
return 0;
