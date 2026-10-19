```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 9V74 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.178 μs** | **0.0033 μs** | **0.0026 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.153 μs** | **0.0097 μs** | **0.0076 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.207 μs** | **0.0046 μs** | **0.0036 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **3.089 μs** | **0.0078 μs** | **0.0069 μs** |
| **Run**    | **encode**    | **CopyTo**             | **3.042 μs** | **0.0045 μs** | **0.0040 μs** |
| **Run**    | **encode**    | **Vectors**            | **3.167 μs** | **0.0054 μs** | **0.0048 μs** |
