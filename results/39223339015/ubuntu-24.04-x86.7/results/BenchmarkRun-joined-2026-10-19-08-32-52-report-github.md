```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 3.58GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    | Median   |
|---------- |------- |----------------- |---------:|----------:|----------:|---------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **3.388 μs** | **0.0662 μs** | **0.0736 μs** | **3.420 μs** |
| X86Encode | Run    | F32 1 avx2       | 3.272 μs | 0.0628 μs | 0.1254 μs | 3.341 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.231 μs** | **0.0358 μs** | **0.0689 μs** | **2.255 μs** |
| X86Encode | Run    | F32 1 DwordChain | 4.143 μs | 0.3569 μs | 1.0525 μs | 3.473 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.207 μs** | **0.2203 μs** | **0.6496 μs** | **1.833 μs** |
| X86Encode | Run    | F32 1 sse        | 4.800 μs | 0.0699 μs | 0.0654 μs | 4.822 μs |
