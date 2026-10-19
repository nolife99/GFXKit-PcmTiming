```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Size     | Method           | Mean        | Error     | StdDev   |
|------- |--------- |----------------- |------------:|----------:|---------:|
| **Run**    | **1-2048**   | **AlignedStores**    |    **26.99 ns** |  **0.136 ns** | **0.120 ns** |
| **Run**    | **1-2048**   | **BufferMemoryCopy** |    **37.87 ns** |  **0.048 ns** | **0.038 ns** |
| **Run**    | **1-2048**   | **MemoryCopy**       |    **27.97 ns** |  **0.034 ns** | **0.030 ns** |
| **Run**    | **1024**     | **AlignedStores**    |    **23.00 ns** |  **0.049 ns** | **0.044 ns** |
| **Run**    | **1024**     | **BufferMemoryCopy** |    **32.59 ns** |  **0.083 ns** | **0.070 ns** |
| **Run**    | **1024**     | **MemoryCopy**       |    **23.69 ns** |  **0.195 ns** | **0.152 ns** |
| **Run**    | **129**      | **AlignedStores**    |    **11.88 ns** |  **0.029 ns** | **0.024 ns** |
| **Run**    | **129**      | **BufferMemoryCopy** |    **12.71 ns** |  **0.010 ns** | **0.008 ns** |
| **Run**    | **129**      | **MemoryCopy**       |    **13.24 ns** |  **0.031 ns** | **0.024 ns** |
| **Run**    | **129-4096** | **AlignedStores**    |    **36.85 ns** |  **0.035 ns** | **0.031 ns** |
| **Run**    | **129-4096** | **BufferMemoryCopy** |    **52.94 ns** |  **0.267 ns** | **0.223 ns** |
| **Run**    | **129-4096** | **MemoryCopy**       |    **37.26 ns** |  **0.067 ns** | **0.060 ns** |
| **Run**    | **129-512**  | **AlignedStores**    |    **19.77 ns** |  **0.180 ns** | **0.159 ns** |
| **Run**    | **129-512**  | **BufferMemoryCopy** |    **29.24 ns** |  **0.289 ns** | **0.241 ns** |
| **Run**    | **129-512**  | **MemoryCopy**       |    **20.97 ns** |  **0.068 ns** | **0.060 ns** |
| **Run**    | **160**      | **AlignedStores**    |    **13.40 ns** |  **0.030 ns** | **0.026 ns** |
| **Run**    | **160**      | **BufferMemoryCopy** |    **12.96 ns** |  **0.037 ns** | **0.033 ns** |
| **Run**    | **160**      | **MemoryCopy**       |    **14.12 ns** |  **0.051 ns** | **0.043 ns** |
| **Run**    | **16384**    | **AlignedStores**    |   **214.92 ns** |  **2.057 ns** | **1.606 ns** |
| **Run**    | **16384**    | **BufferMemoryCopy** |   **334.32 ns** |  **1.060 ns** | **0.885 ns** |
| **Run**    | **16384**    | **MemoryCopy**       |   **215.29 ns** |  **0.348 ns** | **0.291 ns** |
| **Run**    | **192**      | **AlignedStores**    |    **13.17 ns** |  **0.032 ns** | **0.027 ns** |
| **Run**    | **192**      | **BufferMemoryCopy** |    **14.41 ns** |  **0.112 ns** | **0.087 ns** |
| **Run**    | **192**      | **MemoryCopy**       |    **14.28 ns** |  **0.048 ns** | **0.040 ns** |
| **Run**    | **2048**     | **AlignedStores**    |    **32.98 ns** |  **0.074 ns** | **0.061 ns** |
| **Run**    | **2048**     | **BufferMemoryCopy** |    **49.42 ns** |  **0.197 ns** | **0.174 ns** |
| **Run**    | **2048**     | **MemoryCopy**       |    **33.81 ns** |  **0.066 ns** | **0.055 ns** |
| **Run**    | **255**      | **AlignedStores**    |    **13.49 ns** |  **0.019 ns** | **0.015 ns** |
| **Run**    | **255**      | **BufferMemoryCopy** |    **15.10 ns** |  **0.043 ns** | **0.036 ns** |
| **Run**    | **255**      | **MemoryCopy**       |    **14.36 ns** |  **0.060 ns** | **0.047 ns** |
| **Run**    | **256**      | **AlignedStores**    |    **13.65 ns** |  **0.028 ns** | **0.023 ns** |
| **Run**    | **256**      | **BufferMemoryCopy** |    **22.19 ns** |  **0.101 ns** | **0.084 ns** |
| **Run**    | **256**      | **MemoryCopy**       |    **16.29 ns** |  **0.042 ns** | **0.035 ns** |
| **Run**    | **257**      | **AlignedStores**    |    **13.60 ns** |  **0.018 ns** | **0.014 ns** |
| **Run**    | **257**      | **BufferMemoryCopy** |    **22.49 ns** |  **0.083 ns** | **0.073 ns** |
| **Run**    | **257**      | **MemoryCopy**       |    **14.62 ns** |  **0.028 ns** | **0.025 ns** |
| **Run**    | **320**      | **AlignedStores**    |    **15.56 ns** |  **0.026 ns** | **0.020 ns** |
| **Run**    | **320**      | **BufferMemoryCopy** |    **22.76 ns** |  **0.059 ns** | **0.052 ns** |
| **Run**    | **320**      | **MemoryCopy**       |    **16.32 ns** |  **0.023 ns** | **0.018 ns** |
| **Run**    | **384**      | **AlignedStores**    |    **15.01 ns** |  **0.017 ns** | **0.013 ns** |
| **Run**    | **384**      | **BufferMemoryCopy** |    **23.33 ns** |  **0.171 ns** | **0.151 ns** |
| **Run**    | **384**      | **MemoryCopy**       |    **16.13 ns** |  **0.096 ns** | **0.080 ns** |
| **Run**    | **4096**     | **AlignedStores**    |    **54.21 ns** |  **0.099 ns** | **0.082 ns** |
| **Run**    | **4096**     | **BufferMemoryCopy** |    **63.44 ns** |  **0.091 ns** | **0.081 ns** |
| **Run**    | **4096**     | **MemoryCopy**       |    **54.27 ns** |  **0.100 ns** | **0.088 ns** |
| **Run**    | **448**      | **AlignedStores**    |    **17.62 ns** |  **0.012 ns** | **0.010 ns** |
| **Run**    | **448**      | **BufferMemoryCopy** |    **24.24 ns** |  **0.050 ns** | **0.047 ns** |
| **Run**    | **448**      | **MemoryCopy**       |    **18.70 ns** |  **0.034 ns** | **0.028 ns** |
| **Run**    | **511**      | **AlignedStores**    |    **17.82 ns** |  **0.027 ns** | **0.021 ns** |
| **Run**    | **511**      | **BufferMemoryCopy** |    **24.76 ns** |  **0.045 ns** | **0.035 ns** |
| **Run**    | **511**      | **MemoryCopy**       |    **20.48 ns** |  **0.055 ns** | **0.046 ns** |
| **Run**    | **512**      | **AlignedStores**    |    **17.69 ns** |  **0.228 ns** | **0.178 ns** |
| **Run**    | **512**      | **BufferMemoryCopy** |    **24.85 ns** |  **0.036 ns** | **0.028 ns** |
| **Run**    | **512**      | **MemoryCopy**       |    **18.28 ns** |  **0.095 ns** | **0.074 ns** |
| **Run**    | **65536**    | **AlignedStores**    | **1,333.78 ns** | **12.196 ns** | **9.521 ns** |
| **Run**    | **65536**    | **BufferMemoryCopy** | **1,604.19 ns** |  **3.425 ns** | **2.860 ns** |
| **Run**    | **65536**    | **MemoryCopy**       | **1,332.11 ns** |  **3.180 ns** | **2.482 ns** |
| **Run**    | **768**      | **AlignedStores**    |    **20.28 ns** |  **0.037 ns** | **0.033 ns** |
| **Run**    | **768**      | **BufferMemoryCopy** |    **28.26 ns** |  **0.085 ns** | **0.071 ns** |
| **Run**    | **768**      | **MemoryCopy**       |    **21.13 ns** |  **0.058 ns** | **0.045 ns** |
