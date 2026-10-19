```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
Intel Xeon 6973P-C 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **2.983 μs** | **0.0122 μs** | **0.0095 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.844 μs** | **0.0403 μs** | **0.0336 μs** |
| **Run**    | **decode**    | **Vectors**            | **2.667 μs** | **0.0479 μs** | **0.0424 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.454 μs** | **0.0250 μs** | **0.0595 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.194 μs** | **0.0761 μs** | **0.0906 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.998 μs** | **0.0349 μs** | **0.0310 μs** |
