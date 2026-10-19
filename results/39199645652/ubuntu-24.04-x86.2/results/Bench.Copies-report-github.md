```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.337 μs** | **0.0061 μs** | **0.0054 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **4.226 μs** | **0.0060 μs** | **0.0053 μs** |
| **Run**    | **decode**    | **Vectors**            | **38.081 μs** | **0.0910 μs** | **0.0806 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **5.118 μs** | **0.0094 μs** | **0.0078 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.999 μs** | **0.0080 μs** | **0.0075 μs** |
| **Run**    | **encode**    | **Vectors**            | **38.269 μs** | **0.1020 μs** | **0.0904 μs** |
