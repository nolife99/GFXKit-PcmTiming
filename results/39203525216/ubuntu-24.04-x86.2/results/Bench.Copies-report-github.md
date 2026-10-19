```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V45 2.60GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Direction | Size  | Method     | Mean        | Error     | StdDev    |
|------- |---------- |------ |----------- |------------:|----------:|----------:|
| **Run**    | **decode**    | **4096**  | **CopyTo**     |  **2,467.4 ns** |  **17.42 ns** |  **14.54 ns** |
| **Run**    | **decode**    | **4096**  | **Tuned256**   |  **1,323.5 ns** |   **8.01 ns** |   **7.10 ns** |
| **Run**    | **decode**    | **4096**  | **Tuned256NT** | **15,923.2 ns** |  **50.31 ns** |  **42.01 ns** |
| **Run**    | **decode**    | **4096**  | **Tuned512**   |  **1,149.9 ns** |   **3.89 ns** |   **3.64 ns** |
| **Run**    | **decode**    | **4096**  | **Tuned512NT** | **14,623.8 ns** | **267.65 ns** | **250.36 ns** |
| **Run**    | **decode**    | **65536** | **CopyTo**     |    **993.9 ns** |   **2.63 ns** |   **2.05 ns** |
| **Run**    | **decode**    | **65536** | **Tuned256**   |  **1,299.2 ns** |  **12.17 ns** |  **10.16 ns** |
| **Run**    | **decode**    | **65536** | **Tuned256NT** |  **3,528.7 ns** |  **19.03 ns** |  **15.89 ns** |
| **Run**    | **decode**    | **65536** | **Tuned512**   |  **1,305.3 ns** |   **6.74 ns** |   **6.30 ns** |
| **Run**    | **decode**    | **65536** | **Tuned512NT** |  **3,570.5 ns** |  **55.36 ns** |  **51.78 ns** |
| **Run**    | **encode**    | **4096**  | **CopyTo**     |  **2,000.6 ns** |   **4.64 ns** |   **4.34 ns** |
| **Run**    | **encode**    | **4096**  | **Tuned256**   |  **1,361.0 ns** |   **3.94 ns** |   **3.50 ns** |
| **Run**    | **encode**    | **4096**  | **Tuned256NT** | **10,623.6 ns** |  **58.19 ns** |  **54.43 ns** |
| **Run**    | **encode**    | **4096**  | **Tuned512**   |  **1,107.6 ns** |  **10.77 ns** |   **9.55 ns** |
| **Run**    | **encode**    | **4096**  | **Tuned512NT** | **10,816.0 ns** |  **30.23 ns** |  **28.28 ns** |
| **Run**    | **encode**    | **65536** | **CopyTo**     |  **1,004.2 ns** |   **1.21 ns** |   **1.01 ns** |
| **Run**    | **encode**    | **65536** | **Tuned256**   |  **1,292.7 ns** |  **14.59 ns** |  **12.93 ns** |
| **Run**    | **encode**    | **65536** | **Tuned256NT** |  **3,575.8 ns** |   **9.45 ns** |   **8.84 ns** |
| **Run**    | **encode**    | **65536** | **Tuned512**   |  **1,297.9 ns** |  **18.82 ns** |  **16.68 ns** |
| **Run**    | **encode**    | **65536** | **Tuned512NT** |  **3,657.2 ns** |  **42.03 ns** |  **35.10 ns** |
