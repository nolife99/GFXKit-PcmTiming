```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.22GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    |
|---------- |------- |----------------- |---------:|----------:|----------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.648 μs** | **0.0274 μs** | **0.0243 μs** |
| X86Encode | Run    | F32 1 avx2       | 2.593 μs | 0.0256 μs | 0.0214 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **1.230 μs** | **0.0246 μs** | **0.0502 μs** |
| X86Encode | Run    | F32 1 DwordChain | 1.328 μs | 0.0231 μs | 0.0526 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **1.295 μs** | **0.0151 μs** | **0.0327 μs** |
| X86Encode | Run    | F32 1 sse        | 2.726 μs | 0.0295 μs | 0.0247 μs |
