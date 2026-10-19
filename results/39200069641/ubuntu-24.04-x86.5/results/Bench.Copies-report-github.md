```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **4.429 μs** | **0.0055 μs** | **0.0051 μs** |
| **Run**    | **decode**    | **CopyTo**             | **4.156 μs** | **0.0070 μs** | **0.0058 μs** |
| **Run**    | **decode**    | **Vectors**            | **5.459 μs** | **0.0152 μs** | **0.0143 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.534 μs** | **0.0111 μs** | **0.0093 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.338 μs** | **0.0040 μs** | **0.0037 μs** |
| **Run**    | **encode**    | **Vectors**            | **2.599 μs** | **0.0304 μs** | **0.0586 μs** |
