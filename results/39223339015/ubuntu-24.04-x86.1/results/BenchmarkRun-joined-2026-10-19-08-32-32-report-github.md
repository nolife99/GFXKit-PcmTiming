```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 4.17GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **1.563 μs** | **0.0076 μs** | **0.0156 μs** |
| X86Encode | Run    | F32 1 avx2       | 4.125 μs | 0.0079 μs | 0.0070 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.976 μs** | **0.0087 μs** | **0.0081 μs** |
| X86Encode | Run    | F32 1 DwordChain | 5.164 μs | 0.0142 μs | 0.0111 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.952 μs** | **0.0044 μs** | **0.0041 μs** |
| X86Encode | Run    | F32 1 sse        | 4.199 μs | 0.0074 μs | 0.0069 μs |
