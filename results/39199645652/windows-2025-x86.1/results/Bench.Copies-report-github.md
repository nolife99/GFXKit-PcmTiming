```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.354 μs** | **0.0085 μs** | **0.0067 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **4.194 μs** | **0.0073 μs** | **0.0065 μs** |
| **Run**    | **decode**    | **Vectors**            | **16.465 μs** | **0.0247 μs** | **0.0206 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **5.031 μs** | **0.0551 μs** | **0.0430 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.678 μs** | **0.0078 μs** | **0.0065 μs** |
| **Run**    | **encode**    | **Vectors**            | **17.536 μs** | **0.0272 μs** | **0.0255 μs** |
