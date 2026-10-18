using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

var config = ManualConfig.CreateEmpty()
    .AddJob(Job.Default.WithIterationTime(TimeInterval.FromMilliseconds(100)).WithEvaluateOverhead(false))
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default)
    .AddExporter(CsvExporter.Default, MarkdownExporter.GitHub)
    .WithOptions(ConfigOptions.JoinSummary);
BenchmarkSwitcher.FromAssembly(typeof(Bench.Decode).Assembly).Run(args, config);
