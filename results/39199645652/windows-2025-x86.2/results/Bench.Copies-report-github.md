```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    | Median    |
|------- |---------- |------------------- |----------:|----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.222 μs** | **0.0043 μs** | **0.0033 μs** |  **3.221 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.130 μs** | **0.0136 μs** | **0.0114 μs** |  **3.126 μs** |
| **Run**    | **decode**    | **Vectors**            |  **5.387 μs** | **1.9268 μs** | **5.6811 μs** |  **1.700 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **2.449 μs** | **0.2026 μs** | **0.5973 μs** |  **2.023 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **1.996 μs** | **0.0398 μs** | **0.0955 μs** |  **1.953 μs** |
| **Run**    | **encode**    | **Vectors**            | **13.249 μs** | **0.0204 μs** | **0.0160 μs** | **13.246 μs** |
