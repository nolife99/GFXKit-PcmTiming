```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.894 μs** | **0.0461 μs** | **0.0385 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.809 μs** | **0.0090 μs** | **0.0075 μs** |
| **Run**    | **decode**    | **Vectors**            | **29.510 μs** | **0.1941 μs** | **0.1515 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **4.582 μs** | **0.0168 μs** | **0.0140 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.487 μs** | **0.0110 μs** | **0.0086 μs** |
| **Run**    | **encode**    | **Vectors**            | **29.642 μs** | **0.1337 μs** | **0.1044 μs** |
