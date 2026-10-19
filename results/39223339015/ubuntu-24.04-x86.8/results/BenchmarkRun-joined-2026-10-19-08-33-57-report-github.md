```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.59GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **3.597 μs** | **0.0077 μs** | **0.0068 μs** |
| X86Encode | Run    | F32 1 avx2       | 3.630 μs | 0.0033 μs | 0.0031 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **3.461 μs** | **0.0274 μs** | **0.0256 μs** |
| X86Encode | Run    | F32 1 DwordChain | 3.703 μs | 0.0164 μs | 0.0145 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **3.603 μs** | **0.0021 μs** | **0.0020 μs** |
| X86Encode | Run    | F32 1 sse        | 3.589 μs | 0.0379 μs | 0.0355 μs |
