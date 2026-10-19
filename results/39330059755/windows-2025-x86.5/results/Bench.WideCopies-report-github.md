```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33438/24H2/2024Update/HudsonValley) (Hyper-V)
AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Size     | Method           | Mean        | Error    | StdDev   |
|------- |--------- |----------------- |------------:|---------:|---------:|
| **Run**    | **1-2048**   | **AlignedStores**    |    **27.32 ns** | **0.060 ns** | **0.053 ns** |
| **Run**    | **1-2048**   | **BufferMemoryCopy** |    **37.71 ns** | **0.066 ns** | **0.055 ns** |
| **Run**    | **1-2048**   | **Control**          |    **27.52 ns** | **0.324 ns** | **0.271 ns** |
| **Run**    | **1-2048**   | **MemoryCopy**       |    **28.01 ns** | **0.080 ns** | **0.067 ns** |
| **Run**    | **1024**     | **AlignedStores**    |    **24.14 ns** | **0.034 ns** | **0.027 ns** |
| **Run**    | **1024**     | **BufferMemoryCopy** |    **32.37 ns** | **0.079 ns** | **0.066 ns** |
| **Run**    | **1024**     | **Control**          |    **23.67 ns** | **0.023 ns** | **0.019 ns** |
| **Run**    | **1024**     | **MemoryCopy**       |    **23.88 ns** | **0.187 ns** | **0.146 ns** |
| **Run**    | **129**      | **AlignedStores**    |    **12.69 ns** | **0.034 ns** | **0.027 ns** |
| **Run**    | **129**      | **BufferMemoryCopy** |    **13.21 ns** | **0.037 ns** | **0.034 ns** |
| **Run**    | **129**      | **Control**          |    **13.10 ns** | **0.021 ns** | **0.018 ns** |
| **Run**    | **129**      | **MemoryCopy**       |    **13.78 ns** | **0.038 ns** | **0.032 ns** |
| **Run**    | **129-4096** | **AlignedStores**    |    **36.73 ns** | **0.076 ns** | **0.068 ns** |
| **Run**    | **129-4096** | **BufferMemoryCopy** |    **55.70 ns** | **0.133 ns** | **0.104 ns** |
| **Run**    | **129-4096** | **Control**          |    **37.09 ns** | **0.065 ns** | **0.054 ns** |
| **Run**    | **129-4096** | **MemoryCopy**       |    **37.49 ns** | **0.032 ns** | **0.025 ns** |
| **Run**    | **129-512**  | **AlignedStores**    |    **20.23 ns** | **0.138 ns** | **0.107 ns** |
| **Run**    | **129-512**  | **BufferMemoryCopy** |    **28.92 ns** | **0.074 ns** | **0.058 ns** |
| **Run**    | **129-512**  | **Control**          |    **20.67 ns** | **0.108 ns** | **0.096 ns** |
| **Run**    | **129-512**  | **MemoryCopy**       |    **21.49 ns** | **0.068 ns** | **0.053 ns** |
| **Run**    | **160**      | **AlignedStores**    |    **14.18 ns** | **0.055 ns** | **0.043 ns** |
| **Run**    | **160**      | **BufferMemoryCopy** |    **13.37 ns** | **0.049 ns** | **0.038 ns** |
| **Run**    | **160**      | **Control**          |    **13.77 ns** | **0.016 ns** | **0.013 ns** |
| **Run**    | **160**      | **MemoryCopy**       |    **14.35 ns** | **0.055 ns** | **0.046 ns** |
| **Run**    | **16384**    | **AlignedStores**    |   **216.29 ns** | **0.327 ns** | **0.256 ns** |
| **Run**    | **16384**    | **BufferMemoryCopy** |   **337.14 ns** | **4.554 ns** | **3.803 ns** |
| **Run**    | **16384**    | **Control**          |   **218.86 ns** | **0.865 ns** | **0.766 ns** |
| **Run**    | **16384**    | **MemoryCopy**       |   **216.78 ns** | **0.707 ns** | **0.552 ns** |
| **Run**    | **192**      | **AlignedStores**    |    **14.48 ns** | **0.012 ns** | **0.009 ns** |
| **Run**    | **192**      | **BufferMemoryCopy** |    **14.30 ns** | **0.023 ns** | **0.018 ns** |
| **Run**    | **192**      | **Control**          |    **13.86 ns** | **0.034 ns** | **0.028 ns** |
| **Run**    | **192**      | **MemoryCopy**       |    **16.04 ns** | **0.045 ns** | **0.040 ns** |
| **Run**    | **2048**     | **AlignedStores**    |    **33.85 ns** | **0.039 ns** | **0.031 ns** |
| **Run**    | **2048**     | **BufferMemoryCopy** |    **49.63 ns** | **0.145 ns** | **0.135 ns** |
| **Run**    | **2048**     | **Control**          |    **34.38 ns** | **0.057 ns** | **0.050 ns** |
| **Run**    | **2048**     | **MemoryCopy**       |    **34.52 ns** | **0.132 ns** | **0.110 ns** |
| **Run**    | **255**      | **AlignedStores**    |    **13.93 ns** | **0.068 ns** | **0.053 ns** |
| **Run**    | **255**      | **BufferMemoryCopy** |    **15.08 ns** | **0.018 ns** | **0.015 ns** |
| **Run**    | **255**      | **Control**          |    **15.14 ns** | **0.024 ns** | **0.019 ns** |
| **Run**    | **255**      | **MemoryCopy**       |    **14.59 ns** | **0.025 ns** | **0.021 ns** |
| **Run**    | **256**      | **AlignedStores**    |    **14.04 ns** | **0.037 ns** | **0.033 ns** |
| **Run**    | **256**      | **BufferMemoryCopy** |    **21.73 ns** | **0.039 ns** | **0.032 ns** |
| **Run**    | **256**      | **Control**          |    **14.86 ns** | **0.048 ns** | **0.037 ns** |
| **Run**    | **256**      | **MemoryCopy**       |    **14.40 ns** | **0.024 ns** | **0.021 ns** |
| **Run**    | **257**      | **AlignedStores**    |    **14.94 ns** | **0.037 ns** | **0.031 ns** |
| **Run**    | **257**      | **BufferMemoryCopy** |    **21.65 ns** | **0.063 ns** | **0.053 ns** |
| **Run**    | **257**      | **Control**          |    **14.96 ns** | **0.037 ns** | **0.029 ns** |
| **Run**    | **257**      | **MemoryCopy**       |    **17.34 ns** | **0.020 ns** | **0.017 ns** |
| **Run**    | **320**      | **AlignedStores**    |    **16.10 ns** | **0.044 ns** | **0.034 ns** |
| **Run**    | **320**      | **BufferMemoryCopy** |    **22.44 ns** | **0.185 ns** | **0.145 ns** |
| **Run**    | **320**      | **Control**          |    **15.90 ns** | **0.042 ns** | **0.033 ns** |
| **Run**    | **320**      | **MemoryCopy**       |    **16.17 ns** | **0.028 ns** | **0.023 ns** |
| **Run**    | **384**      | **AlignedStores**    |    **15.99 ns** | **0.017 ns** | **0.013 ns** |
| **Run**    | **384**      | **BufferMemoryCopy** |    **23.19 ns** | **0.032 ns** | **0.026 ns** |
| **Run**    | **384**      | **Control**          |    **16.01 ns** | **0.027 ns** | **0.021 ns** |
| **Run**    | **384**      | **MemoryCopy**       |    **19.13 ns** | **0.024 ns** | **0.018 ns** |
| **Run**    | **4096**     | **AlignedStores**    |    **54.33 ns** | **0.067 ns** | **0.056 ns** |
| **Run**    | **4096**     | **BufferMemoryCopy** |    **60.84 ns** | **0.095 ns** | **0.085 ns** |
| **Run**    | **4096**     | **Control**          |    **55.14 ns** | **0.121 ns** | **0.101 ns** |
| **Run**    | **4096**     | **MemoryCopy**       |    **54.25 ns** | **0.384 ns** | **0.300 ns** |
| **Run**    | **448**      | **AlignedStores**    |    **18.16 ns** | **0.031 ns** | **0.026 ns** |
| **Run**    | **448**      | **BufferMemoryCopy** |    **23.95 ns** | **0.042 ns** | **0.033 ns** |
| **Run**    | **448**      | **Control**          |    **18.16 ns** | **0.015 ns** | **0.012 ns** |
| **Run**    | **448**      | **MemoryCopy**       |    **18.59 ns** | **0.171 ns** | **0.151 ns** |
| **Run**    | **511**      | **AlignedStores**    |    **20.04 ns** | **0.062 ns** | **0.051 ns** |
| **Run**    | **511**      | **BufferMemoryCopy** |    **24.50 ns** | **0.032 ns** | **0.025 ns** |
| **Run**    | **511**      | **Control**          |    **18.13 ns** | **0.021 ns** | **0.018 ns** |
| **Run**    | **511**      | **MemoryCopy**       |    **18.30 ns** | **0.081 ns** | **0.064 ns** |
| **Run**    | **512**      | **AlignedStores**    |    **18.54 ns** | **0.035 ns** | **0.031 ns** |
| **Run**    | **512**      | **BufferMemoryCopy** |    **24.48 ns** | **0.037 ns** | **0.029 ns** |
| **Run**    | **512**      | **Control**          |    **18.32 ns** | **0.048 ns** | **0.040 ns** |
| **Run**    | **512**      | **MemoryCopy**       |    **18.55 ns** | **0.015 ns** | **0.012 ns** |
| **Run**    | **65536**    | **AlignedStores**    | **1,332.53 ns** | **6.145 ns** | **5.131 ns** |
| **Run**    | **65536**    | **BufferMemoryCopy** | **1,599.54 ns** | **6.170 ns** | **4.817 ns** |
| **Run**    | **65536**    | **Control**          | **1,331.48 ns** | **1.460 ns** | **1.219 ns** |
| **Run**    | **65536**    | **MemoryCopy**       | **1,331.50 ns** | **2.185 ns** | **1.706 ns** |
| **Run**    | **768**      | **AlignedStores**    |    **21.03 ns** | **0.054 ns** | **0.045 ns** |
| **Run**    | **768**      | **BufferMemoryCopy** |    **28.40 ns** | **0.051 ns** | **0.040 ns** |
| **Run**    | **768**      | **Control**          |    **21.00 ns** | **0.026 ns** | **0.020 ns** |
| **Run**    | **768**      | **MemoryCopy**       |    **21.61 ns** | **0.023 ns** | **0.018 ns** |
