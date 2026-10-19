```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Method             | Mean      | Error     | StdDev    | Median    |
|------- |---------- |------------------- |----------:|----------:|----------:|----------:|
| **Run**    | **decode**    | **CopyBlockUnaligned** |  **3.291 μs** | **0.2613 μs** | **0.7703 μs** |  **3.005 μs** |
| **Run**    | **decode**    | **CopyTo**             |  **3.771 μs** | **0.0738 μs** | **0.0821 μs** |  **3.741 μs** |
| **Run**    | **decode**    | **Vectors**            | **29.787 μs** | **0.0455 μs** | **0.0355 μs** | **29.787 μs** |
| **Run**    | **encode**    | **CopyBlockUnaligned** |  **4.761 μs** | **0.0313 μs** | **0.0261 μs** |  **4.751 μs** |
| **Run**    | **encode**    | **CopyTo**             |  **4.468 μs** | **0.0075 μs** | **0.0067 μs** |  **4.467 μs** |
| **Run**    | **encode**    | **Vectors**            | **29.818 μs** | **0.0721 μs** | **0.0563 μs** | **29.807 μs** |
