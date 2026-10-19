```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    | Median   |
|------- |---------- |------------------- |---------:|----------:|----------:|---------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.893 μs** | **0.0078 μs** | **0.0061 μs** | **3.892 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.691 μs** | **0.0284 μs** | **0.0252 μs** | **3.679 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.888 μs** | **0.0119 μs** | **0.0106 μs** | **3.886 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.353 μs** | **0.1823 μs** | **0.5142 μs** | **3.051 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.580 μs** | **0.0316 μs** | **0.0264 μs** | **4.565 μs** |
| **Run**    | **encode**    | **Vectors**            | **3.988 μs** | **0.3330 μs** | **0.9818 μs** | **3.558 μs** |
