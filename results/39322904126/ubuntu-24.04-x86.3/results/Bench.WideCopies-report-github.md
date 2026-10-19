```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 2.87GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3
  Job-NJPIMG : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v3

EvaluateOverhead=False  Affinity=0100  IterationTime=100ms  

```
| Method | Size     | Method           | Mean         | Error     | StdDev    |
|------- |--------- |----------------- |-------------:|----------:|----------:|
| **Run**    | **1-2048**   | **AlignedStores**    |    **18.350 ns** | **0.0869 ns** | **0.1961 ns** |
| **Run**    | **1-2048**   | **BufferMemoryCopy** |    **39.428 ns** | **0.2216 ns** | **0.1850 ns** |
| **Run**    | **1-2048**   | **MemoryCopy**       |    **28.587 ns** | **0.1074 ns** | **0.0952 ns** |
| **Run**    | **1024**     | **AlignedStores**    |    **26.756 ns** | **0.0396 ns** | **0.0351 ns** |
| **Run**    | **1024**     | **BufferMemoryCopy** |    **33.768 ns** | **0.1215 ns** | **0.1136 ns** |
| **Run**    | **1024**     | **MemoryCopy**       |    **28.613 ns** | **0.0134 ns** | **0.0112 ns** |
| **Run**    | **129**      | **AlignedStores**    |    **12.202 ns** | **0.0371 ns** | **0.0329 ns** |
| **Run**    | **129**      | **BufferMemoryCopy** |    **13.598 ns** | **0.0286 ns** | **0.0253 ns** |
| **Run**    | **129**      | **MemoryCopy**       |    **14.125 ns** | **0.0190 ns** | **0.0178 ns** |
| **Run**    | **129-4096** | **AlignedStores**    |    **40.343 ns** | **0.0750 ns** | **0.0701 ns** |
| **Run**    | **129-4096** | **BufferMemoryCopy** |    **59.834 ns** | **0.1740 ns** | **0.1627 ns** |
| **Run**    | **129-4096** | **MemoryCopy**       |    **40.288 ns** | **0.0457 ns** | **0.0428 ns** |
| **Run**    | **129-512**  | **AlignedStores**    |    **16.523 ns** | **0.0272 ns** | **0.0227 ns** |
| **Run**    | **129-512**  | **BufferMemoryCopy** |    **29.033 ns** | **0.1747 ns** | **0.1549 ns** |
| **Run**    | **129-512**  | **MemoryCopy**       |    **17.763 ns** | **0.0518 ns** | **0.0459 ns** |
| **Run**    | **160**      | **AlignedStores**    |    **14.125 ns** | **0.0210 ns** | **0.0196 ns** |
| **Run**    | **160**      | **BufferMemoryCopy** |    **14.206 ns** | **0.0111 ns** | **0.0093 ns** |
| **Run**    | **160**      | **MemoryCopy**       |    **15.201 ns** | **0.0144 ns** | **0.0120 ns** |
| **Run**    | **16384**    | **AlignedStores**    |   **243.744 ns** | **0.6562 ns** | **0.5123 ns** |
| **Run**    | **16384**    | **BufferMemoryCopy** |   **307.566 ns** | **0.3076 ns** | **0.2878 ns** |
| **Run**    | **16384**    | **MemoryCopy**       |   **245.782 ns** | **2.2428 ns** | **2.0979 ns** |
| **Run**    | **192**      | **AlignedStores**    |    **14.108 ns** | **0.0091 ns** | **0.0081 ns** |
| **Run**    | **192**      | **BufferMemoryCopy** |    **17.587 ns** | **0.0132 ns** | **0.0117 ns** |
| **Run**    | **192**      | **MemoryCopy**       |     **8.123 ns** | **0.0368 ns** | **0.0761 ns** |
| **Run**    | **2048**     | **AlignedStores**    |    **38.102 ns** | **0.0472 ns** | **0.0369 ns** |
| **Run**    | **2048**     | **BufferMemoryCopy** |    **53.648 ns** | **0.1169 ns** | **0.1093 ns** |
| **Run**    | **2048**     | **MemoryCopy**       |    **38.754 ns** | **0.0226 ns** | **0.0189 ns** |
| **Run**    | **255**      | **AlignedStores**    |    **13.950 ns** | **0.0074 ns** | **0.0062 ns** |
| **Run**    | **255**      | **BufferMemoryCopy** |    **17.182 ns** | **0.0206 ns** | **0.0172 ns** |
| **Run**    | **255**      | **MemoryCopy**       |    **15.313 ns** | **0.0209 ns** | **0.0164 ns** |
| **Run**    | **256**      | **AlignedStores**    |    **14.532 ns** | **0.0116 ns** | **0.0091 ns** |
| **Run**    | **256**      | **BufferMemoryCopy** |     **8.197 ns** | **0.1286 ns** | **0.2849 ns** |
| **Run**    | **256**      | **MemoryCopy**       |    **15.932 ns** | **0.1203 ns** | **0.1067 ns** |
| **Run**    | **257**      | **AlignedStores**    |    **14.633 ns** | **0.0300 ns** | **0.0266 ns** |
| **Run**    | **257**      | **BufferMemoryCopy** |    **21.910 ns** | **0.3940 ns** | **0.3492 ns** |
| **Run**    | **257**      | **MemoryCopy**       |    **16.022 ns** | **0.0182 ns** | **0.0162 ns** |
| **Run**    | **320**      | **AlignedStores**    |     **8.627 ns** | **0.0730 ns** | **0.1458 ns** |
| **Run**    | **320**      | **BufferMemoryCopy** |    **22.805 ns** | **0.0659 ns** | **0.0617 ns** |
| **Run**    | **320**      | **MemoryCopy**       |    **18.038 ns** | **0.0151 ns** | **0.0133 ns** |
| **Run**    | **384**      | **AlignedStores**    |    **16.621 ns** | **0.0096 ns** | **0.0080 ns** |
| **Run**    | **384**      | **BufferMemoryCopy** |    **23.819 ns** | **0.2274 ns** | **0.2127 ns** |
| **Run**    | **384**      | **MemoryCopy**       |    **17.805 ns** | **0.0124 ns** | **0.0110 ns** |
| **Run**    | **4096**     | **AlignedStores**    |    **71.976 ns** | **0.1169 ns** | **0.1093 ns** |
| **Run**    | **4096**     | **BufferMemoryCopy** |    **87.613 ns** | **0.0650 ns** | **0.0576 ns** |
| **Run**    | **4096**     | **MemoryCopy**       |    **61.257 ns** | **0.0472 ns** | **0.0394 ns** |
| **Run**    | **448**      | **AlignedStores**    |    **21.406 ns** | **0.0125 ns** | **0.0111 ns** |
| **Run**    | **448**      | **BufferMemoryCopy** |    **24.693 ns** | **0.0533 ns** | **0.0498 ns** |
| **Run**    | **448**      | **MemoryCopy**       |    **20.356 ns** | **0.0201 ns** | **0.0188 ns** |
| **Run**    | **511**      | **AlignedStores**    |    **10.305 ns** | **0.0856 ns** | **0.1710 ns** |
| **Run**    | **511**      | **BufferMemoryCopy** |    **25.444 ns** | **0.1800 ns** | **0.1684 ns** |
| **Run**    | **511**      | **MemoryCopy**       |    **20.118 ns** | **0.0591 ns** | **0.0524 ns** |
| **Run**    | **512**      | **AlignedStores**    |    **19.588 ns** | **0.0106 ns** | **0.0088 ns** |
| **Run**    | **512**      | **BufferMemoryCopy** |    **25.752 ns** | **0.1360 ns** | **0.1272 ns** |
| **Run**    | **512**      | **MemoryCopy**       |    **20.783 ns** | **0.0526 ns** | **0.0492 ns** |
| **Run**    | **65536**    | **AlignedStores**    | **1,515.809 ns** | **0.5355 ns** | **0.4472 ns** |
| **Run**    | **65536**    | **BufferMemoryCopy** | **1,751.650 ns** | **1.3276 ns** | **1.0365 ns** |
| **Run**    | **65536**    | **MemoryCopy**       | **1,509.755 ns** | **1.5054 ns** | **1.3345 ns** |
| **Run**    | **768**      | **AlignedStores**    |    **23.292 ns** | **0.0255 ns** | **0.0213 ns** |
| **Run**    | **768**      | **BufferMemoryCopy** |    **29.295 ns** | **0.0920 ns** | **0.0861 ns** |
| **Run**    | **768**      | **MemoryCopy**       |    **24.081 ns** | **0.0288 ns** | **0.0270 ns** |
