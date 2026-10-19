```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case       | Mean     | Error     | StdDev    |
|---------- |------- |----------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2** | **4.417 μs** | **0.0164 μs** | **0.0128 μs** |
| X86Encode | Run    | F32 1 avx2 | 5.056 μs | 0.0070 μs | 0.0055 μs |
| **X86Decode** | **Run**    | **F32 1 sse**  | **4.413 μs** | **0.0363 μs** | **0.0322 μs** |
| X86Encode | Run    | F32 1 sse  | 5.190 μs | 0.0106 μs | 0.0089 μs |
