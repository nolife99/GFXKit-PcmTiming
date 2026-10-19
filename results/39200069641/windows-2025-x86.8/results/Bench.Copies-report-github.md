```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.902 μs** | **0.0147 μs** | **0.0115 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.715 μs** | **0.0213 μs** | **0.0178 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.881 μs** | **0.0184 μs** | **0.0154 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.596 μs** | **0.0104 μs** | **0.0097 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.435 μs** | **0.0107 μs** | **0.0089 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.606 μs** | **0.0059 μs** | **0.0046 μs** |
