```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    | Median   |
|---------- |------- |----------------- |---------:|----------:|----------:|---------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **2.897 μs** | **0.0406 μs** | **0.0360 μs** | **2.879 μs** |
| X86Encode | Run    | F32 1 avx2       | 1.343 μs | 0.0091 μs | 0.0206 μs | 1.342 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **1.595 μs** | **0.0318 μs** | **0.0844 μs** | **1.563 μs** |
| X86Encode | Run    | F32 1 DwordChain | 2.474 μs | 0.0453 μs | 0.0354 μs | 2.480 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **1.383 μs** | **0.0407 μs** | **0.1087 μs** | **1.344 μs** |
| X86Encode | Run    | F32 1 sse        | 1.379 μs | 0.0252 μs | 0.0629 μs | 1.355 μs |
