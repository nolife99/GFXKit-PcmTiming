```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 4.17GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **2.708 μs** | **0.0292 μs** | **0.0243 μs** |
| **Run**    | **decode**    | **CopyTo**             | **2.494 μs** | **0.0137 μs** | **0.0122 μs** |
| **Run**    | **decode**    | **Vectors**            | **1.465 μs** | **0.0133 μs** | **0.0111 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **2.203 μs** | **0.0211 μs** | **0.0164 μs** |
| **Run**    | **encode**    | **CopyTo**             | **2.122 μs** | **0.0370 μs** | **0.0346 μs** |
| **Run**    | **encode**    | **Vectors**            | **1.564 μs** | **0.0189 μs** | **0.0177 μs** |
