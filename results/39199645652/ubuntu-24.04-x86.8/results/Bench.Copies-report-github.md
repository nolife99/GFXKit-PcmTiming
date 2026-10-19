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
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **4.384 μs** | **0.0148 μs** | **0.0116 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **4.239 μs** | **0.0077 μs** | **0.0065 μs** |
| **Run**    | **decode**    | **Vectors**            | **38.289 μs** | **0.0590 μs** | **0.0523 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **5.076 μs** | **0.0074 μs** | **0.0069 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.969 μs** | **0.0041 μs** | **0.0037 μs** |
| **Run**    | **encode**    | **Vectors**            | **38.841 μs** | **0.0501 μs** | **0.0419 μs** |
