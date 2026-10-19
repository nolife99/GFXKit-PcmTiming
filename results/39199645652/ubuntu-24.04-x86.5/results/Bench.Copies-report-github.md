```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    |
|------- |---------- |------------------- |----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.330 μs** | **0.0216 μs** | **0.0192 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.237 μs** | **0.0067 μs** | **0.0059 μs** |
| **Run**    | **decode**    | **Vectors**            | **28.275 μs** | **0.0402 μs** | **0.0376 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **3.495 μs** | **0.0036 μs** | **0.0030 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **3.424 μs** | **0.0043 μs** | **0.0039 μs** |
| **Run**    | **encode**    | **Vectors**            | **15.285 μs** | **0.0201 μs** | **0.0188 μs** |
