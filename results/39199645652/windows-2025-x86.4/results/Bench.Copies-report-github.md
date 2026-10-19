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
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.989 μs** | **0.0294 μs** | **0.0245 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.792 μs** | **0.0560 μs** | **0.0468 μs** |
| **Run**    | **decode**    | **Vectors**            | **28.844 μs** | **0.1789 μs** | **0.1397 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **4.622 μs** | **0.0115 μs** | **0.0096 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.484 μs** | **0.0079 μs** | **0.0070 μs** |
| **Run**    | **encode**    | **Vectors**            | **29.995 μs** | **0.2560 μs** | **0.2270 μs** |
