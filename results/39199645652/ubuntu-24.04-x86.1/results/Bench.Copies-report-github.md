```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.375 μs** | **0.0115 μs** | **0.0102 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **4.217 μs** | **0.0153 μs** | **0.0136 μs** |
| **Run**    | **decode**    | **Vectors**            | **38.705 μs** | **0.0474 μs** | **0.0443 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **5.041 μs** | **0.0076 μs** | **0.0067 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.963 μs** | **0.0051 μs** | **0.0046 μs** |
| **Run**    | **encode**    | **Vectors**            | **38.636 μs** | **0.0467 μs** | **0.0414 μs** |
