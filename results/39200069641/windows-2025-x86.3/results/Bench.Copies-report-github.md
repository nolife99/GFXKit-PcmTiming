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
| **Run**    | **decode**    | **CopyBlockUnaligned** | **4.109 μs** | **0.0231 μs** | **0.0181 μs** | **4.103 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.858 μs** | **0.0175 μs** | **0.0155 μs** | **3.851 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.976 μs** | **0.0072 μs** | **0.0056 μs** | **3.977 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.075 μs** | **0.0578 μs** | **0.1397 μs** | **3.015 μs** |
| **Run**    | **encode**    | **CopyTo**             | **3.085 μs** | **0.0979 μs** | **0.2647 μs** | **2.970 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.628 μs** | **0.0055 μs** | **0.0046 μs** | **4.627 μs** |
