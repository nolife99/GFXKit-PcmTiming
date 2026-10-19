```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **2.635 μs** | **0.0522 μs** | **0.0558 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **2.440 μs** | **0.0461 μs** | **0.0431 μs** |
| **Run**    | **decode**    | **Vectors**            |  **1.060 μs** | **0.0148 μs** | **0.0305 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **2.133 μs** | **0.0407 μs** | **0.0435 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **2.031 μs** | **0.0406 μs** | **0.0451 μs** |
| **Run**    | **encode**    | **Vectors**            | **11.964 μs** | **0.2040 μs** | **0.1909 μs** |
