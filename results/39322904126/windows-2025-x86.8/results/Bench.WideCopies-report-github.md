```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Size     | Method           | Mean        | Error     | StdDev    |
|------- |--------- |----------------- |------------:|----------:|----------:|
| **Run**    | **1-2048**   | **AlignedStores**    |    **27.10 ns** |  **0.252 ns** |  **0.197 ns** |
| **Run**    | **1-2048**   | **BufferMemoryCopy** |    **37.93 ns** |  **0.151 ns** |  **0.118 ns** |
| **Run**    | **1-2048**   | **MemoryCopy**       |    **28.14 ns** |  **0.075 ns** |  **0.067 ns** |
| **Run**    | **1024**     | **AlignedStores**    |    **22.62 ns** |  **0.036 ns** |  **0.028 ns** |
| **Run**    | **1024**     | **BufferMemoryCopy** |    **32.39 ns** |  **0.096 ns** |  **0.080 ns** |
| **Run**    | **1024**     | **MemoryCopy**       |    **28.67 ns** |  **0.031 ns** |  **0.024 ns** |
| **Run**    | **129**      | **AlignedStores**    |    **14.38 ns** |  **0.020 ns** |  **0.016 ns** |
| **Run**    | **129**      | **BufferMemoryCopy** |    **12.78 ns** |  **0.024 ns** |  **0.023 ns** |
| **Run**    | **129**      | **MemoryCopy**       |    **13.13 ns** |  **0.047 ns** |  **0.036 ns** |
| **Run**    | **129-4096** | **AlignedStores**    |    **36.81 ns** |  **0.051 ns** |  **0.045 ns** |
| **Run**    | **129-4096** | **BufferMemoryCopy** |    **56.97 ns** |  **0.096 ns** |  **0.085 ns** |
| **Run**    | **129-4096** | **MemoryCopy**       |    **37.93 ns** |  **0.120 ns** |  **0.101 ns** |
| **Run**    | **129-512**  | **AlignedStores**    |    **20.99 ns** |  **0.143 ns** |  **0.127 ns** |
| **Run**    | **129-512**  | **BufferMemoryCopy** |    **29.14 ns** |  **0.045 ns** |  **0.035 ns** |
| **Run**    | **129-512**  | **MemoryCopy**       |    **20.93 ns** |  **0.068 ns** |  **0.053 ns** |
| **Run**    | **160**      | **AlignedStores**    |    **13.01 ns** |  **0.017 ns** |  **0.014 ns** |
| **Run**    | **160**      | **BufferMemoryCopy** |    **13.43 ns** |  **0.159 ns** |  **0.133 ns** |
| **Run**    | **160**      | **MemoryCopy**       |    **14.53 ns** |  **0.033 ns** |  **0.028 ns** |
| **Run**    | **16384**    | **AlignedStores**    |   **215.57 ns** |  **0.349 ns** |  **0.291 ns** |
| **Run**    | **16384**    | **BufferMemoryCopy** |   **333.48 ns** |  **0.659 ns** |  **0.551 ns** |
| **Run**    | **16384**    | **MemoryCopy**       |   **216.01 ns** |  **0.581 ns** |  **0.454 ns** |
| **Run**    | **192**      | **AlignedStores**    |    **13.19 ns** |  **0.025 ns** |  **0.020 ns** |
| **Run**    | **192**      | **BufferMemoryCopy** |    **14.43 ns** |  **0.055 ns** |  **0.046 ns** |
| **Run**    | **192**      | **MemoryCopy**       |    **14.60 ns** |  **0.042 ns** |  **0.035 ns** |
| **Run**    | **2048**     | **AlignedStores**    |    **32.77 ns** |  **0.054 ns** |  **0.045 ns** |
| **Run**    | **2048**     | **BufferMemoryCopy** |    **49.66 ns** |  **0.109 ns** |  **0.085 ns** |
| **Run**    | **2048**     | **MemoryCopy**       |    **33.82 ns** |  **0.048 ns** |  **0.037 ns** |
| **Run**    | **255**      | **AlignedStores**    |    **13.17 ns** |  **0.110 ns** |  **0.086 ns** |
| **Run**    | **255**      | **BufferMemoryCopy** |    **15.11 ns** |  **0.088 ns** |  **0.069 ns** |
| **Run**    | **255**      | **MemoryCopy**       |    **14.29 ns** |  **0.037 ns** |  **0.033 ns** |
| **Run**    | **256**      | **AlignedStores**    |    **13.31 ns** |  **0.219 ns** |  **0.171 ns** |
| **Run**    | **256**      | **BufferMemoryCopy** |    **21.84 ns** |  **0.106 ns** |  **0.094 ns** |
| **Run**    | **256**      | **MemoryCopy**       |    **15.32 ns** |  **0.033 ns** |  **0.028 ns** |
| **Run**    | **257**      | **AlignedStores**    |    **13.81 ns** |  **0.275 ns** |  **0.215 ns** |
| **Run**    | **257**      | **BufferMemoryCopy** |    **21.83 ns** |  **0.076 ns** |  **0.063 ns** |
| **Run**    | **257**      | **MemoryCopy**       |    **15.95 ns** |  **0.109 ns** |  **0.085 ns** |
| **Run**    | **320**      | **AlignedStores**    |    **15.02 ns** |  **0.020 ns** |  **0.016 ns** |
| **Run**    | **320**      | **BufferMemoryCopy** |    **14.01 ns** |  **0.198 ns** |  **0.439 ns** |
| **Run**    | **320**      | **MemoryCopy**       |    **16.04 ns** |  **0.029 ns** |  **0.026 ns** |
| **Run**    | **384**      | **AlignedStores**    |    **15.76 ns** |  **0.015 ns** |  **0.013 ns** |
| **Run**    | **384**      | **BufferMemoryCopy** |    **23.38 ns** |  **0.444 ns** |  **0.371 ns** |
| **Run**    | **384**      | **MemoryCopy**       |    **16.10 ns** |  **0.017 ns** |  **0.014 ns** |
| **Run**    | **4096**     | **AlignedStores**    |    **54.79 ns** |  **0.067 ns** |  **0.052 ns** |
| **Run**    | **4096**     | **BufferMemoryCopy** |    **62.26 ns** |  **0.093 ns** |  **0.072 ns** |
| **Run**    | **4096**     | **MemoryCopy**       |    **65.21 ns** |  **0.091 ns** |  **0.076 ns** |
| **Run**    | **448**      | **AlignedStores**    |    **17.59 ns** |  **0.037 ns** |  **0.032 ns** |
| **Run**    | **448**      | **BufferMemoryCopy** |    **24.21 ns** |  **0.054 ns** |  **0.045 ns** |
| **Run**    | **448**      | **MemoryCopy**       |    **18.03 ns** |  **0.042 ns** |  **0.037 ns** |
| **Run**    | **511**      | **AlignedStores**    |    **17.54 ns** |  **0.023 ns** |  **0.018 ns** |
| **Run**    | **511**      | **BufferMemoryCopy** |    **24.53 ns** |  **0.087 ns** |  **0.068 ns** |
| **Run**    | **511**      | **MemoryCopy**       |    **18.04 ns** |  **0.030 ns** |  **0.025 ns** |
| **Run**    | **512**      | **AlignedStores**    |    **17.61 ns** |  **0.231 ns** |  **0.193 ns** |
| **Run**    | **512**      | **BufferMemoryCopy** |    **24.62 ns** |  **0.061 ns** |  **0.048 ns** |
| **Run**    | **512**      | **MemoryCopy**       |    **18.26 ns** |  **0.019 ns** |  **0.015 ns** |
| **Run**    | **65536**    | **AlignedStores**    | **1,422.25 ns** |  **1.511 ns** |  **1.180 ns** |
| **Run**    | **65536**    | **BufferMemoryCopy** | **1,684.86 ns** | **18.678 ns** | **15.597 ns** |
| **Run**    | **65536**    | **MemoryCopy**       | **1,359.92 ns** |  **2.226 ns** |  **1.738 ns** |
| **Run**    | **768**      | **AlignedStores**    |    **20.18 ns** |  **0.031 ns** |  **0.024 ns** |
| **Run**    | **768**      | **BufferMemoryCopy** |    **28.24 ns** |  **0.060 ns** |  **0.050 ns** |
| **Run**    | **768**      | **MemoryCopy**       |    **21.05 ns** |  **0.041 ns** |  **0.034 ns** |
