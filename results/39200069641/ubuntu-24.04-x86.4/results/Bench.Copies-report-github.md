```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **2.120 μs** | **0.0028 μs** | **0.0025 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.042 μs** | **0.0020 μs** | **0.0017 μs** |
| **Run**    | **decode**    | **Vectors**            | **2.011 μs** | **0.0086 μs** | **0.0081 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.661 μs** | **0.0040 μs** | **0.0035 μs** |
| **Run**    | **encode**    | **CopyTo**             | **3.627 μs** | **0.1208 μs** | **0.3523 μs** |
| **Run**    | **encode**    | **Vectors**            | **3.199 μs** | **0.0103 μs** | **0.0096 μs** |
