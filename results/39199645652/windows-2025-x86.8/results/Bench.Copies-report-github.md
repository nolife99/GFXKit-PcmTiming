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
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.045 μs** | **0.0078 μs** | **0.0065 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.726 μs** | **0.0094 μs** | **0.0083 μs** |
| **Run**    | **decode**    | **Vectors**            | **28.788 μs** | **0.0597 μs** | **0.0529 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **4.565 μs** | **0.0059 μs** | **0.0046 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.451 μs** | **0.0115 μs** | **0.0096 μs** |
| **Run**    | **encode**    | **Vectors**            | **29.688 μs** | **0.0928 μs** | **0.0823 μs** |
