```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 7763 2.77GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Size     | Method           | Mean        | Error    | StdDev   |
|------- |--------- |----------------- |------------:|---------:|---------:|
| **Run**    | **1-2048**   | **AlignedStores**    |    **18.37 ns** | **0.317 ns** | **0.611 ns** |
| **Run**    | **1-2048**   | **BufferMemoryCopy** |    **43.69 ns** | **0.168 ns** | **0.149 ns** |
| **Run**    | **1-2048**   | **Control**          |    **30.02 ns** | **0.037 ns** | **0.033 ns** |
| **Run**    | **1-2048**   | **MemoryCopy**       |    **28.84 ns** | **0.078 ns** | **0.061 ns** |
| **Run**    | **1024**     | **AlignedStores**    |    **28.79 ns** | **0.029 ns** | **0.027 ns** |
| **Run**    | **1024**     | **BufferMemoryCopy** |    **37.07 ns** | **0.060 ns** | **0.054 ns** |
| **Run**    | **1024**     | **Control**          |    **28.75 ns** | **0.079 ns** | **0.066 ns** |
| **Run**    | **1024**     | **MemoryCopy**       |    **26.67 ns** | **0.036 ns** | **0.030 ns** |
| **Run**    | **129**      | **AlignedStores**    |    **12.52 ns** | **0.016 ns** | **0.013 ns** |
| **Run**    | **129**      | **BufferMemoryCopy** |    **15.00 ns** | **0.055 ns** | **0.049 ns** |
| **Run**    | **129**      | **Control**          |    **14.63 ns** | **0.022 ns** | **0.018 ns** |
| **Run**    | **129**      | **MemoryCopy**       |    **14.27 ns** | **0.024 ns** | **0.023 ns** |
| **Run**    | **129-4096** | **AlignedStores**    |    **41.17 ns** | **0.036 ns** | **0.032 ns** |
| **Run**    | **129-4096** | **BufferMemoryCopy** |    **70.81 ns** | **0.058 ns** | **0.048 ns** |
| **Run**    | **129-4096** | **Control**          |    **39.34 ns** | **0.063 ns** | **0.056 ns** |
| **Run**    | **129-4096** | **MemoryCopy**       |    **38.06 ns** | **0.039 ns** | **0.037 ns** |
| **Run**    | **129-512**  | **AlignedStores**    |    **24.58 ns** | **0.114 ns** | **0.095 ns** |
| **Run**    | **129-512**  | **BufferMemoryCopy** |    **31.78 ns** | **0.038 ns** | **0.036 ns** |
| **Run**    | **129-512**  | **Control**          |    **24.31 ns** | **0.070 ns** | **0.066 ns** |
| **Run**    | **129-512**  | **MemoryCopy**       |    **21.80 ns** | **0.026 ns** | **0.024 ns** |
| **Run**    | **160**      | **AlignedStores**    |    **14.03 ns** | **0.010 ns** | **0.008 ns** |
| **Run**    | **160**      | **BufferMemoryCopy** |    **15.00 ns** | **0.008 ns** | **0.006 ns** |
| **Run**    | **160**      | **Control**          |    **15.16 ns** | **0.063 ns** | **0.049 ns** |
| **Run**    | **160**      | **MemoryCopy**       |    **15.25 ns** | **0.015 ns** | **0.012 ns** |
| **Run**    | **16384**    | **AlignedStores**    |   **219.10 ns** | **0.277 ns** | **0.260 ns** |
| **Run**    | **16384**    | **BufferMemoryCopy** |   **327.30 ns** | **0.434 ns** | **0.406 ns** |
| **Run**    | **16384**    | **Control**          |   **218.85 ns** | **0.460 ns** | **0.359 ns** |
| **Run**    | **16384**    | **MemoryCopy**       |   **220.99 ns** | **0.636 ns** | **0.531 ns** |
| **Run**    | **192**      | **AlignedStores**    |    **14.20 ns** | **0.038 ns** | **0.033 ns** |
| **Run**    | **192**      | **BufferMemoryCopy** |    **17.09 ns** | **0.026 ns** | **0.023 ns** |
| **Run**    | **192**      | **Control**          |    **14.78 ns** | **0.011 ns** | **0.009 ns** |
| **Run**    | **192**      | **MemoryCopy**       |    **15.58 ns** | **0.013 ns** | **0.010 ns** |
| **Run**    | **2048**     | **AlignedStores**    |    **25.81 ns** | **0.444 ns** | **0.927 ns** |
| **Run**    | **2048**     | **BufferMemoryCopy** |    **51.24 ns** | **0.072 ns** | **0.060 ns** |
| **Run**    | **2048**     | **Control**          |    **39.28 ns** | **0.036 ns** | **0.033 ns** |
| **Run**    | **2048**     | **MemoryCopy**       |    **35.01 ns** | **0.120 ns** | **0.094 ns** |
| **Run**    | **255**      | **AlignedStores**    |    **14.79 ns** | **0.026 ns** | **0.023 ns** |
| **Run**    | **255**      | **BufferMemoryCopy** |    **17.93 ns** | **0.043 ns** | **0.036 ns** |
| **Run**    | **255**      | **Control**          |    **14.07 ns** | **0.016 ns** | **0.014 ns** |
| **Run**    | **255**      | **MemoryCopy**       |    **15.98 ns** | **0.017 ns** | **0.014 ns** |
| **Run**    | **256**      | **AlignedStores**    |    **14.96 ns** | **0.020 ns** | **0.017 ns** |
| **Run**    | **256**      | **BufferMemoryCopy** |    **23.85 ns** | **0.079 ns** | **0.062 ns** |
| **Run**    | **256**      | **Control**          |    **14.86 ns** | **0.024 ns** | **0.020 ns** |
| **Run**    | **256**      | **MemoryCopy**       |    **15.39 ns** | **0.011 ns** | **0.009 ns** |
| **Run**    | **257**      | **AlignedStores**    |    **14.76 ns** | **0.015 ns** | **0.012 ns** |
| **Run**    | **257**      | **BufferMemoryCopy** |    **26.80 ns** | **0.084 ns** | **0.065 ns** |
| **Run**    | **257**      | **Control**          |    **14.57 ns** | **0.020 ns** | **0.018 ns** |
| **Run**    | **257**      | **MemoryCopy**       |    **16.01 ns** | **0.014 ns** | **0.011 ns** |
| **Run**    | **320**      | **AlignedStores**    |    **18.17 ns** | **0.024 ns** | **0.022 ns** |
| **Run**    | **320**      | **BufferMemoryCopy** |    **13.08 ns** | **0.033 ns** | **0.071 ns** |
| **Run**    | **320**      | **Control**          |    **18.44 ns** | **0.023 ns** | **0.021 ns** |
| **Run**    | **320**      | **MemoryCopy**       |    **17.39 ns** | **0.024 ns** | **0.021 ns** |
| **Run**    | **384**      | **AlignedStores**    |    **18.31 ns** | **0.039 ns** | **0.034 ns** |
| **Run**    | **384**      | **BufferMemoryCopy** |    **26.35 ns** | **0.072 ns** | **0.068 ns** |
| **Run**    | **384**      | **Control**          |    **18.67 ns** | **0.022 ns** | **0.021 ns** |
| **Run**    | **384**      | **MemoryCopy**       |    **17.96 ns** | **0.026 ns** | **0.023 ns** |
| **Run**    | **4096**     | **AlignedStores**    |    **66.99 ns** | **0.265 ns** | **0.248 ns** |
| **Run**    | **4096**     | **BufferMemoryCopy** |   **103.07 ns** | **0.143 ns** | **0.127 ns** |
| **Run**    | **4096**     | **Control**          |    **58.39 ns** | **0.049 ns** | **0.043 ns** |
| **Run**    | **4096**     | **MemoryCopy**       |    **58.27 ns** | **0.057 ns** | **0.048 ns** |
| **Run**    | **448**      | **AlignedStores**    |    **23.09 ns** | **0.050 ns** | **0.044 ns** |
| **Run**    | **448**      | **BufferMemoryCopy** |    **27.76 ns** | **0.083 ns** | **0.073 ns** |
| **Run**    | **448**      | **Control**          |    **22.70 ns** | **0.021 ns** | **0.018 ns** |
| **Run**    | **448**      | **MemoryCopy**       |    **19.05 ns** | **0.088 ns** | **0.069 ns** |
| **Run**    | **511**      | **AlignedStores**    |    **22.79 ns** | **0.046 ns** | **0.043 ns** |
| **Run**    | **511**      | **BufferMemoryCopy** |    **29.27 ns** | **0.055 ns** | **0.046 ns** |
| **Run**    | **511**      | **Control**          |    **23.42 ns** | **0.010 ns** | **0.008 ns** |
| **Run**    | **511**      | **MemoryCopy**       |    **18.97 ns** | **0.032 ns** | **0.027 ns** |
| **Run**    | **512**      | **AlignedStores**    |    **22.85 ns** | **0.059 ns** | **0.050 ns** |
| **Run**    | **512**      | **BufferMemoryCopy** |    **30.39 ns** | **0.081 ns** | **0.076 ns** |
| **Run**    | **512**      | **Control**          |    **23.42 ns** | **0.018 ns** | **0.016 ns** |
| **Run**    | **512**      | **MemoryCopy**       |    **19.11 ns** | **0.013 ns** | **0.010 ns** |
| **Run**    | **65536**    | **AlignedStores**    | **1,349.88 ns** | **1.032 ns** | **0.915 ns** |
| **Run**    | **65536**    | **BufferMemoryCopy** | **1,617.49 ns** | **7.700 ns** | **6.826 ns** |
| **Run**    | **65536**    | **Control**          | **1,339.38 ns** | **2.540 ns** | **2.121 ns** |
| **Run**    | **65536**    | **MemoryCopy**       | **1,338.69 ns** | **0.814 ns** | **0.680 ns** |
| **Run**    | **768**      | **AlignedStores**    |    **25.74 ns** | **0.017 ns** | **0.015 ns** |
| **Run**    | **768**      | **BufferMemoryCopy** |    **33.10 ns** | **0.018 ns** | **0.016 ns** |
| **Run**    | **768**      | **Control**          |    **24.45 ns** | **0.030 ns** | **0.027 ns** |
| **Run**    | **768**      | **MemoryCopy**       |    **22.12 ns** | **0.015 ns** | **0.012 ns** |
