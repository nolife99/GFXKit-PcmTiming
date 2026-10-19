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
| **Run**    | **decode**    | **CopyBlockUnaligned** | **4.373 μs** | **0.0108 μs** | **0.0101 μs** |
| **Run**    | **decode**    | **CopyTo**             | **4.254 μs** | **0.0128 μs** | **0.0114 μs** |
| **Run**    | **decode**    | **Vectors**            | **5.085 μs** | **0.0240 μs** | **0.0201 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **5.058 μs** | **0.0109 μs** | **0.0102 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.975 μs** | **0.0064 μs** | **0.0054 μs** |
| **Run**    | **encode**    | **Vectors**            | **5.403 μs** | **0.0118 μs** | **0.0092 μs** |
