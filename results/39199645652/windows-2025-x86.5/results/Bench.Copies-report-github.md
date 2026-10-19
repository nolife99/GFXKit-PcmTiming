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
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.011 μs** | **0.0321 μs** | **0.0251 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.833 μs** | **0.0648 μs** | **0.0541 μs** |
| **Run**    | **decode**    | **Vectors**            |  **2.559 μs** | **0.0058 μs** | **0.0115 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **4.612 μs** | **0.0054 μs** | **0.0042 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.474 μs** | **0.0680 μs** | **0.0531 μs** |
| **Run**    | **encode**    | **Vectors**            | **30.623 μs** | **0.0880 μs** | **0.0735 μs** |
