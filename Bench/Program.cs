using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;

var job = Job.Default.WithIterationTime(TimeInterval.FromMilliseconds(100)).WithEvaluateOverhead(false);
var config = ManualConfig.CreateEmpty()
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default)
    .AddExporter(CsvExporter.Default, MarkdownExporter.GitHub)
    .WithOptions(ConfigOptions.JoinSummary);
if (args.Length > 0)
{
    BenchmarkSwitcher.FromAssembly(typeof(Bench.Decode).Assembly).Run(args, config.AddJob(job));
    return;
}

var desktop = OperatingSystem.IsWindows() ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : "";
var folder = Path.Combine(Directory.Exists(desktop) ? desktop : AppContext.BaseDirectory, $"gfxkit-bench-{DateTime.Now:yyyyMMdd-HHmmss}");
Console.WriteLine("Timing GFXKit's x86 PCM kernels. This takes about 20 minutes; leave the computer alone and plugged in until it finishes.");
Console.WriteLine();
try
{
    if (OperatingSystem.IsWindows()) Win32.KeepAwake();
    if (Environment.ProcessorCount > 2 && (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()))
        Process.GetCurrentProcess().ProcessorAffinity = (nint)(1 << 2);
    BenchmarkSwitcher.FromAssembly(typeof(Bench.Decode).Assembly)
        .Run(["--filter", "Bench.X86*"], config.AddJob(job.WithToolchain(InProcessEmitToolchain.Instance)).WithArtifactsPath(folder));
    var zip = folder + ".zip";
    ZipFile.CreateFromDirectory(folder, zip);
    Console.WriteLine();
    Console.WriteLine($"Done. Send this file to Dylan: {zip}");
    if (OperatingSystem.IsWindows() && Win32.OwnsConsole()) Process.Start("explorer.exe", $"/select,\"{zip}\"");
}
catch (Exception e)
{
    Console.WriteLine();
    Console.WriteLine($"The run failed. Send Dylan a screenshot of this window, or the folder {folder} if it exists.");
    Console.WriteLine(e);
}
if (OperatingSystem.IsWindows() && Win32.OwnsConsole())
{
    Console.WriteLine("Press Enter to close.");
    Console.ReadLine();
}

static class Win32
{
    [DllImport("kernel32")]
    static extern uint SetThreadExecutionState(uint flags);

    [DllImport("kernel32")]
    static extern uint GetConsoleProcessList(uint[] list, uint count);

    public static void KeepAwake() => SetThreadExecutionState(0x80000000 | 0x00000001);

    public static bool OwnsConsole() => GetConsoleProcessList(new uint[2], 2) == 1;
}
