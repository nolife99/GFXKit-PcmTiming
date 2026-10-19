```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean     | Error     | StdDev    |
|------- |---------- |------------------- |---------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** | **3.913 μs** | **0.0249 μs** | **0.0221 μs** |
| **Run**    | **decode**    | **CopyTo**             | **3.700 μs** | **0.0320 μs** | **0.0268 μs** |
| **Run**    | **decode**    | **Vectors**            | **3.882 μs** | **0.0080 μs** | **0.0067 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** | **4.600 μs** | **0.0763 μs** | **0.0596 μs** |
| **Run**    | **encode**    | **CopyTo**             | **4.496 μs** | **0.0094 μs** | **0.0073 μs** |
| **Run**    | **encode**    | **Vectors**            | **4.727 μs** | **0.0093 μs** | **0.0073 μs** |
