```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.55GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.592 μs** | **0.0026 μs** | **0.0021 μs** |
| X86Encode | Run    | F32 1 avx2       | 2.550 μs | 0.0014 μs | 0.0013 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.096 μs** | **0.0035 μs** | **0.0029 μs** |
| X86Encode | Run    | F32 1 DwordChain | 2.171 μs | 0.0013 μs | 0.0012 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.606 μs** | **0.0022 μs** | **0.0020 μs** |
| X86Encode | Run    | F32 1 sse        | 1.275 μs | 0.0168 μs | 0.0324 μs |
