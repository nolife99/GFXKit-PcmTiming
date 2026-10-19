```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.54GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.326 μs** | **0.0194 μs** | **0.0162 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.271 μs** | **0.0058 μs** | **0.0051 μs** |
| **Run**    | **decode**    | **Vectors**            | **27.952 μs** | **0.0615 μs** | **0.0513 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **3.480 μs** | **0.0058 μs** | **0.0048 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **3.375 μs** | **0.0038 μs** | **0.0034 μs** |
| **Run**    | **encode**    | **Vectors**            |  **1.763 μs** | **0.0350 μs** | **0.0700 μs** |
