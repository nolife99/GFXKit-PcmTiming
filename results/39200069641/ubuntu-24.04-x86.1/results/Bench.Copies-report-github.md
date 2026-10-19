```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Intel Xeon 6973P-C 4.20GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **2.117 μs** | **0.0020 μs** | **0.0015 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.394 μs** | **0.0037 μs** | **0.0035 μs** |
| **Run**    | **decode**    | **Vectors**            | **2.017 μs** | **0.0087 μs** | **0.0077 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.662 μs** | **0.0228 μs** | **0.0190 μs** |
| **Run**    | **encode**    | **CopyTo**             | **3.615 μs** | **0.0051 μs** | **0.0040 μs** |
| **Run**    | **encode**    | **Vectors**            | **3.227 μs** | **0.0120 μs** | **0.0107 μs** |
