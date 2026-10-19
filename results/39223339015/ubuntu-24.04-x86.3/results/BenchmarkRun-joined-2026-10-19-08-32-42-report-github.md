```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **3.628 μs** | **0.0056 μs** | **0.0050 μs** |
| X86Encode | Run    | F32 1 avx2       | 3.631 μs | 0.0025 μs | 0.0023 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **1.934 μs** | **0.0151 μs** | **0.0302 μs** |
| X86Encode | Run    | F32 1 DwordChain | 3.654 μs | 0.0030 μs | 0.0025 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **3.583 μs** | **0.0043 μs** | **0.0036 μs** |
| X86Encode | Run    | F32 1 sse        | 3.661 μs | 0.0036 μs | 0.0030 μs |
