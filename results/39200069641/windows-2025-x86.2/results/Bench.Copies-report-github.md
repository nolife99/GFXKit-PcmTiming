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
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.951 μs** | **0.0392 μs** | **0.0327 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.722 μs** | **0.0062 μs** | **0.0052 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.918 μs** | **0.0102 μs** | **0.0090 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.603 μs** | **0.0217 μs** | **0.0192 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.457 μs** | **0.0217 μs** | **0.0169 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.618 μs** | **0.0476 μs** | **0.0371 μs** |
