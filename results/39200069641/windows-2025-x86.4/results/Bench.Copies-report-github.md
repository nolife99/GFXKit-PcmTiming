```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    | Median   |
|------- |---------- |------------------- |---------:|----------:|----------:|---------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.287 μs** | **0.3580 μs** | **1.0554 μs** | **2.735 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.713 μs** | **0.1207 μs** | **0.3263 μs** | **2.575 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.129 μs** | **0.1152 μs** | **0.3323 μs** | **3.020 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.031 μs** | **0.3224 μs** | **0.9507 μs** | **3.313 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.676 μs** | **0.0052 μs** | **0.0044 μs** | **4.675 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.775 μs** | **0.0096 μs** | **0.0085 μs** | **4.773 μs** |
