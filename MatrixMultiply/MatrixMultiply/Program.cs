// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using static MatrixMultiply.Multiplication;
using static MatrixMultiply.ReadFile;

var stopwatch = new Stopwatch();
var n = 12;

Console.WriteLine(" Mean single |  Mean parallel | Deviation single | Deviation parallel");
for (var l = 1; l <= 50; l++)
{
    var i = l;
    if (i == 50)
    {
        i = 1000;
    }

    var size = 10 * i;
    var path = "../../../../MatrixMultiply.Tests/Tests/test" + i + ".txt";
    var singleTimes = new double[n];
    var parallelTimes = new double[n];

    for (var j = 0; j < n; j++)
    {
        stopwatch.Start();
        var matrixSingle = SingleThreadedMultiplication(
            ReadFileMatrix(path),
            ReadFileMatrix(path));
        stopwatch.Stop();
        singleTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
        stopwatch.Start();
        var matrixMulti = MultiThreadedMultiplication(
            ReadFileMatrix(path),
            ReadFileMatrix(path));
        stopwatch.Stop();
        parallelTimes[j] = stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
    }

    var singleMeanValue = (double)singleTimes.Sum() / n;
    var parallelMeanValue = (double)parallelTimes.Sum() / n;

    var sum = singleTimes.Sum(time => Math.Pow(time - singleMeanValue, 2));
    var parallelSum = parallelTimes.Sum(time => Math.Pow(time - parallelMeanValue, 2));

    var singleStandardDeviation = Math.Sqrt(sum / n);
    var parallelStandardDeviation = Math.Sqrt(parallelSum / n);
    i = 999;

    Console.WriteLine($"{i + 1} x {i + 1} | {singleMeanValue:f2} ms | {parallelMeanValue:f2} ms | {singleStandardDeviation:f2} ms | {parallelStandardDeviation:f2} ms");
}