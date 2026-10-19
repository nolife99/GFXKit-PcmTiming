```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    | Median   |
|------- |---------- |------------------- |---------:|----------:|----------:|---------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **1.589 μs** | **0.1151 μs** | **0.3393 μs** | **1.364 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.091 μs** | **0.0348 μs** | **0.0308 μs** | **2.097 μs** |
| **Run**    | **decode**    | **Vectors**            | **8.771 μs** | **0.1749 μs** | **0.1718 μs** | **8.714 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **1.996 μs** | **0.0399 μs** | **0.0832 μs** | **1.980 μs** |
| **Run**    | **encode**    | **CopyTo**             | **2.055 μs** | **0.0071 μs** | **0.0063 μs** | **2.053 μs** |
| **Run**    | **encode**    | **Vectors**            | **9.011 μs** | **0.1351 μs** | **0.1128 μs** | **9.022 μs** |
