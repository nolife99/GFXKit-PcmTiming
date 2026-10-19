```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    | Median   |
|---------- |------- |----------------- |---------:|----------:|----------:|---------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.954 μs** | **0.3607 μs** | **1.0637 μs** | **2.398 μs** |
| X86Encode | Run    | F32 1 avx2       | 4.281 μs | 0.0737 μs | 0.0653 μs | 4.311 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.783 μs** | **0.3342 μs** | **0.9853 μs** | **2.113 μs** |
| X86Encode | Run    | F32 1 DwordChain | 4.360 μs | 0.0376 μs | 0.0333 μs | 4.367 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **2.096 μs** | **0.0417 μs** | **0.0925 μs** | **2.079 μs** |
| X86Encode | Run    | F32 1 sse        | 4.189 μs | 0.0691 μs | 0.0646 μs | 4.195 μs |
