```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.290 μs** | **0.0035 μs** | **0.0031 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.238 μs** | **0.0032 μs** | **0.0030 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.143 μs** | **0.0032 μs** | **0.0027 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.446 μs** | **0.0030 μs** | **0.0026 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.370 μs** | **0.0080 μs** | **0.0067 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.012 μs** | **0.0028 μs** | **0.0026 μs** |
