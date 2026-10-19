```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    | Median    |
|------- |---------- |------------------- |----------:|----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **1.829 μs** | **0.0236 μs** | **0.0477 μs** |  **1.810 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **2.291 μs** | **0.0372 μs** | **0.0329 μs** |  **2.277 μs** |
| **Run**    | **decode**    | **Vectors**            | **10.692 μs** | **0.1033 μs** | **0.0916 μs** | **10.669 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **2.044 μs** | **0.0109 μs** | **0.0091 μs** |  **2.043 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **1.415 μs** | **0.0192 μs** | **0.0412 μs** |  **1.404 μs** |
| **Run**    | **encode**    | **Vectors**            | **11.530 μs** | **0.1194 μs** | **0.1117 μs** | **11.528 μs** |
