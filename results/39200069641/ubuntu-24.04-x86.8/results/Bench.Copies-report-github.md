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
| **Run**    | **decode**    | **CopyBlockUnaligned** | **4.299 μs** | **0.0072 μs** | **0.0067 μs** |
| **Run**    | **decode**    | **CopyTo**             | **4.214 μs** | **0.0052 μs** | **0.0046 μs** |
| **Run**    | **decode**    | **Vectors**            | **5.109 μs** | **0.0135 μs** | **0.0120 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **5.062 μs** | **0.0088 μs** | **0.0082 μs** |
| **Run**    | **encode**    | **CopyTo**             | **5.037 μs** | **0.0580 μs** | **0.0484 μs** |
| **Run**    | **encode**    | **Vectors**            | **5.485 μs** | **0.0152 μs** | **0.0135 μs** |
