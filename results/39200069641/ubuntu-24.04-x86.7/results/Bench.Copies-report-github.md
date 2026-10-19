```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **4.339 μs** | **0.0129 μs** | **0.0114 μs** |
| **Run**    | **decode**    | **CopyTo**             | **4.220 μs** | **0.0070 μs** | **0.0058 μs** |
| **Run**    | **decode**    | **Vectors**            | **5.091 μs** | **0.0239 μs** | **0.0224 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **5.051 μs** | **0.0130 μs** | **0.0108 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.971 μs** | **0.0095 μs** | **0.0085 μs** |
| **Run**    | **encode**    | **Vectors**            | **5.395 μs** | **0.0147 μs** | **0.0137 μs** |
