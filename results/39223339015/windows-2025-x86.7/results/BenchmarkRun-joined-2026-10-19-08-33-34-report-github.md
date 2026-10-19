```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.841 μs** | **0.0511 μs** | **0.0453 μs** |
| X86Encode | Run    | F32 1 avx2       | 2.840 μs | 0.0435 μs | 0.0386 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **1.581 μs** | **0.0306 μs** | **0.0796 μs** |
| X86Encode | Run    | F32 1 DwordChain | 2.509 μs | 0.0206 μs | 0.0202 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **1.884 μs** | **0.1594 μs** | **0.4521 μs** |
| X86Encode | Run    | F32 1 sse        | 2.853 μs | 0.0262 μs | 0.0219 μs |
