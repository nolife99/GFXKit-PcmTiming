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
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.709 μs** | **0.0075 μs** | **0.0067 μs** |
| X86Encode | Run    | F32 1 avx2       | 2.719 μs | 0.0280 μs | 0.0234 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.373 μs** | **0.0420 μs** | **0.0350 μs** |
| X86Encode | Run    | F32 1 DwordChain | 2.391 μs | 0.0054 μs | 0.0048 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.700 μs** | **0.0066 μs** | **0.0058 μs** |
| X86Encode | Run    | F32 1 sse        | 2.751 μs | 0.0541 μs | 0.0623 μs |
