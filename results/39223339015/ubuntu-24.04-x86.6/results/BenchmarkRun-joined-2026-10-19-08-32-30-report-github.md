```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 3.98GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.975 μs** | **0.0258 μs** | **0.0241 μs** |
| X86Encode | Run    | F32 1 avx2       | 4.233 μs | 0.0230 μs | 0.0215 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **3.126 μs** | **0.0505 μs** | **0.0422 μs** |
| X86Encode | Run    | F32 1 DwordChain | 5.173 μs | 0.0361 μs | 0.0320 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.962 μs** | **0.0282 μs** | **0.0263 μs** |
| X86Encode | Run    | F32 1 sse        | 4.195 μs | 0.0480 μs | 0.0375 μs |
