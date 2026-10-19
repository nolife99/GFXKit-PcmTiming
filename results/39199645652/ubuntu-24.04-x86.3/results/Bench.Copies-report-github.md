```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.69GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.331 μs** | **0.0082 μs** | **0.0073 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.238 μs** | **0.0031 μs** | **0.0026 μs** |
| **Run**    | **decode**    | **Vectors**            | **14.782 μs** | **0.0340 μs** | **0.0318 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **3.504 μs** | **0.0047 μs** | **0.0044 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **3.361 μs** | **0.0033 μs** | **0.0031 μs** |
| **Run**    | **encode**    | **Vectors**            | **27.989 μs** | **0.0622 μs** | **0.0519 μs** |
