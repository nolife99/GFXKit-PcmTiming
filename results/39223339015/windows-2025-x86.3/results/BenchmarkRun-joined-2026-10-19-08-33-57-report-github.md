```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
INTEL XEON PLATINUM 8573C 2.30GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Type      | Method | Case             | Mean     | Error     | StdDev    | Median   |
|---------- |------- |----------------- |---------:|----------:|----------:|---------:|
| **X86Decode** | **Run**    | **F32 1 avx2**       | **4.037 μs** | **0.0681 μs** | **0.0603 μs** | **4.012 μs** |
| X86Encode | Run    | F32 1 avx2       | 5.542 μs | 0.0602 μs | 0.0534 μs | 5.522 μs |
| **X86Decode** | **Run**    | **F32 1 DwordChain** | **2.647 μs** | **0.2123 μs** | **0.6259 μs** | **2.216 μs** |
| X86Encode | Run    | F32 1 DwordChain | 6.400 μs | 0.0277 μs | 0.0259 μs | 6.403 μs |
| **X86Decode** | **Run**    | **F32 1 sse**        | **3.961 μs** | **0.0493 μs** | **0.0412 μs** | **3.945 μs** |
| X86Encode | Run    | F32 1 sse        | 5.498 μs | 0.0260 μs | 0.0203 μs | 5.495 μs |
