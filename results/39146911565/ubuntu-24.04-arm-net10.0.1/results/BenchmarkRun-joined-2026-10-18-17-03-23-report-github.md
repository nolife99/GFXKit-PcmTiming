```

BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
Neoverse-N2, 4 physical cores
.NET SDK 10.0.402
  [Host]     : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a
  Job-MGUGJU : .NET 10.0.13 (10.0.13, 10.0.1326.50504), Arm64 RyuJIT armv8.0-a

EvaluateOverhead=False  IterationTime=100ms  

```
| Type   | Method     | Format | Channels | Mean       | Error     | StdDev    | Ratio |
|------- |----------- |------- |--------- |-----------:|----------:|----------:|------:|
| **Decode** | **Registers**  | **F32**    | **1**        |   **3.261 μs** | **0.0038 μs** | **0.0033 μs** |  **0.85** |
| Encode | Registers  | F32    | 1        |   2.263 μs | 0.0047 μs | 0.0042 μs |  0.59 |
| Decode | Structures | F32    | 1        |   3.273 μs | 0.0045 μs | 0.0042 μs |  0.85 |
| Encode | Structures | F32    | 1        |   2.279 μs | 0.0084 μs | 0.0075 μs |  0.59 |
| Decode | Library    | F32    | 1        |   3.844 μs | 0.0070 μs | 0.0062 μs |  1.00 |
| Encode | Library    | F32    | 1        |   2.712 μs | 0.0214 μs | 0.0189 μs |  0.71 |
| Decode | Pointers   | F32    | 1        |   3.708 μs | 0.0086 μs | 0.0077 μs |  0.96 |
| Encode | Pointers   | F32    | 1        |   2.526 μs | 0.0153 μs | 0.0136 μs |  0.66 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F32**    | **2**        |   **9.440 μs** | **0.0064 μs** | **0.0059 μs** |  **1.17** |
| Encode | Registers  | F32    | 2        |   6.465 μs | 0.0016 μs | 0.0015 μs |  0.80 |
| Decode | Structures | F32    | 2        |   9.809 μs | 0.0156 μs | 0.0138 μs |  1.22 |
| Encode | Structures | F32    | 2        |   6.854 μs | 0.0081 μs | 0.0072 μs |  0.85 |
| Decode | Library    | F32    | 2        |   8.035 μs | 0.0150 μs | 0.0140 μs |  1.00 |
| Encode | Library    | F32    | 2        |   6.469 μs | 0.0049 μs | 0.0046 μs |  0.81 |
| Decode | Pointers   | F32    | 2        |   8.305 μs | 0.0237 μs | 0.0222 μs |  1.03 |
| Encode | Pointers   | F32    | 2        |   7.218 μs | 0.0020 μs | 0.0019 μs |  0.90 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F32**    | **3**        |  **19.788 μs** | **0.0098 μs** | **0.0087 μs** |  **0.97** |
| Encode | Registers  | F32    | 3        |  14.456 μs | 0.0064 μs | 0.0060 μs |  0.71 |
| Decode | Structures | F32    | 3        |  20.036 μs | 0.0336 μs | 0.0315 μs |  0.98 |
| Encode | Structures | F32    | 3        |  11.160 μs | 0.0081 μs | 0.0072 μs |  0.55 |
| Decode | Library    | F32    | 3        |  20.374 μs | 0.0216 μs | 0.0181 μs |  1.00 |
| Encode | Library    | F32    | 3        |  11.168 μs | 0.0058 μs | 0.0054 μs |  0.55 |
| Decode | Pointers   | F32    | 3        |  19.783 μs | 0.0299 μs | 0.0279 μs |  0.97 |
| Encode | Pointers   | F32    | 3        |   9.888 μs | 0.0034 μs | 0.0032 μs |  0.49 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F32**    | **4**        |  **26.763 μs** | **0.0465 μs** | **0.0435 μs** |  **0.70** |
| Encode | Registers  | F32    | 4        |  18.036 μs | 0.0111 μs | 0.0098 μs |  0.47 |
| Decode | Structures | F32    | 4        |  32.657 μs | 0.0603 μs | 0.0535 μs |  0.85 |
| Encode | Structures | F32    | 4        |  29.582 μs | 0.0159 μs | 0.0132 μs |  0.77 |
| Decode | Library    | F32    | 4        |  38.224 μs | 0.1665 μs | 0.1557 μs |  1.00 |
| Encode | Library    | F32    | 4        |  29.549 μs | 0.0143 μs | 0.0134 μs |  0.77 |
| Decode | Pointers   | F32    | 4        |  37.683 μs | 0.0783 μs | 0.0733 μs |  0.99 |
| Encode | Pointers   | F32    | 4        |  24.971 μs | 0.0083 μs | 0.0074 μs |  0.65 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F32**    | **6**        |  **50.277 μs** | **0.1179 μs** | **0.1045 μs** |  **1.10** |
| Encode | Registers  | F32    | 6        |  37.775 μs | 0.0407 μs | 0.0381 μs |  0.83 |
| Decode | Structures | F32    | 6        |  46.084 μs | 0.3630 μs | 0.3218 μs |  1.01 |
| Encode | Structures | F32    | 6        |  29.124 μs | 0.0467 μs | 0.0414 μs |  0.64 |
| Decode | Library    | F32    | 6        |  45.514 μs | 0.2087 μs | 0.1952 μs |  1.00 |
| Encode | Library    | F32    | 6        |  28.889 μs | 0.0378 μs | 0.0353 μs |  0.63 |
| Decode | Pointers   | F32    | 6        |  50.161 μs | 0.1174 μs | 0.0980 μs |  1.10 |
| Encode | Pointers   | F32    | 6        |  28.713 μs | 0.0461 μs | 0.0431 μs |  0.63 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F32**    | **8**        |  **66.420 μs** | **0.0864 μs** | **0.0766 μs** |  **1.00** |
| Encode | Registers  | F32    | 8        |  50.461 μs | 0.0416 μs | 0.0369 μs |  0.76 |
| Decode | Structures | F32    | 8        |  68.864 μs | 0.1128 μs | 0.0942 μs |  1.03 |
| Encode | Structures | F32    | 8        |  36.137 μs | 0.0535 μs | 0.0500 μs |  0.54 |
| Decode | Library    | F32    | 8        |  66.619 μs | 0.1904 μs | 0.1590 μs |  1.00 |
| Encode | Library    | F32    | 8        |  50.170 μs | 0.0904 μs | 0.0845 μs |  0.75 |
| Decode | Pointers   | F32    | 8        |  68.849 μs | 0.1407 μs | 0.1247 μs |  1.03 |
| Encode | Pointers   | F32    | 8        |  35.853 μs | 0.0529 μs | 0.0495 μs |  0.54 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **1**        |   **7.412 μs** | **0.0086 μs** | **0.0076 μs** |  **1.00** |
| Decode | Structures | F64    | 1        |   7.407 μs | 0.0092 μs | 0.0082 μs |  1.00 |
| Decode | Library    | F64    | 1        |   7.392 μs | 0.0076 μs | 0.0071 μs |  1.00 |
| Decode | Pointers   | F64    | 1        |   9.368 μs | 0.0063 μs | 0.0059 μs |  1.27 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **2**        |  **21.049 μs** | **0.0473 μs** | **0.0443 μs** |  **1.01** |
| Decode | Structures | F64    | 2        |  23.208 μs | 0.0904 μs | 0.0846 μs |  1.12 |
| Decode | Library    | F64    | 2        |  20.808 μs | 0.0410 μs | 0.0383 μs |  1.00 |
| Decode | Pointers   | F64    | 2        |  24.658 μs | 0.0681 μs | 0.0637 μs |  1.19 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **3**        |  **49.464 μs** | **0.3053 μs** | **0.2856 μs** |  **1.02** |
| Decode | Structures | F64    | 3        |  36.550 μs | 0.3915 μs | 0.3662 μs |  0.75 |
| Decode | Library    | F64    | 3        |  48.592 μs | 0.2677 μs | 0.2504 μs |  1.00 |
| Decode | Pointers   | F64    | 3        |  37.310 μs | 0.1720 μs | 0.1609 μs |  0.77 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **4**        |  **51.219 μs** | **0.2451 μs** | **0.2292 μs** |  **0.99** |
| Decode | Structures | F64    | 4        |  60.087 μs | 0.0447 μs | 0.0349 μs |  1.17 |
| Decode | Library    | F64    | 4        |  51.502 μs | 0.1460 μs | 0.1294 μs |  1.00 |
| Decode | Pointers   | F64    | 4        |  71.739 μs | 0.0900 μs | 0.0842 μs |  1.39 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **6**        | **165.991 μs** | **0.4394 μs** | **0.4110 μs** |  **0.99** |
| Decode | Structures | F64    | 6        | 167.030 μs | 0.4882 μs | 0.4567 μs |  1.00 |
| Decode | Library    | F64    | 6        | 166.829 μs | 0.2330 μs | 0.2179 μs |  1.00 |
| Decode | Pointers   | F64    | 6        | 138.096 μs | 0.2665 μs | 0.2493 μs |  0.83 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **F64**    | **8**        | **214.071 μs** | **0.6598 μs** | **0.6172 μs** |  **1.00** |
| Decode | Structures | F64    | 8        | 213.929 μs | 0.6819 μs | 0.5694 μs |  0.99 |
| Decode | Library    | F64    | 8        | 215.088 μs | 0.8875 μs | 0.8301 μs |  1.00 |
| Decode | Pointers   | F64    | 8        | 161.995 μs | 0.2605 μs | 0.2437 μs |  0.75 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **1**        |   **9.207 μs** | **0.0149 μs** | **0.0139 μs** |  **1.00** |
| Encode | Registers  | S16    | 1        |  12.113 μs | 0.0033 μs | 0.0029 μs |  1.31 |
| Decode | Structures | S16    | 1        |   9.227 μs | 0.0123 μs | 0.0115 μs |  1.00 |
| Encode | Structures | S16    | 1        |  12.102 μs | 0.0037 μs | 0.0035 μs |  1.31 |
| Decode | Library    | S16    | 1        |   9.230 μs | 0.0171 μs | 0.0160 μs |  1.00 |
| Encode | Library    | S16    | 1        |  12.101 μs | 0.0048 μs | 0.0043 μs |  1.31 |
| Decode | Pointers   | S16    | 1        |   9.254 μs | 0.0259 μs | 0.0242 μs |  1.00 |
| Encode | Pointers   | S16    | 1        |  12.034 μs | 0.0047 μs | 0.0044 μs |  1.30 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **2**        |  **20.638 μs** | **0.0083 μs** | **0.0078 μs** |  **1.01** |
| Encode | Registers  | S16    | 2        |  25.689 μs | 0.0059 μs | 0.0049 μs |  1.25 |
| Decode | Structures | S16    | 2        |  20.489 μs | 0.0055 μs | 0.0051 μs |  1.00 |
| Encode | Structures | S16    | 2        |  25.674 μs | 0.0093 μs | 0.0087 μs |  1.25 |
| Decode | Library    | S16    | 2        |  20.486 μs | 0.0034 μs | 0.0031 μs |  1.00 |
| Encode | Library    | S16    | 2        |  25.659 μs | 0.0072 μs | 0.0060 μs |  1.25 |
| Decode | Pointers   | S16    | 2        |  19.425 μs | 0.0124 μs | 0.0116 μs |  0.95 |
| Encode | Pointers   | S16    | 2        |  25.551 μs | 0.0066 μs | 0.0058 μs |  1.25 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **3**        |  **35.679 μs** | **0.0129 μs** | **0.0121 μs** |  **1.15** |
| Encode | Registers  | S16    | 3        |  42.089 μs | 0.0178 μs | 0.0148 μs |  1.36 |
| Decode | Structures | S16    | 3        |  30.926 μs | 0.0093 μs | 0.0083 μs |  1.00 |
| Encode | Structures | S16    | 3        |  38.542 μs | 0.0063 μs | 0.0056 μs |  1.25 |
| Decode | Library    | S16    | 3        |  30.920 μs | 0.0122 μs | 0.0114 μs |  1.00 |
| Encode | Library    | S16    | 3        |  38.494 μs | 0.0078 μs | 0.0066 μs |  1.24 |
| Decode | Pointers   | S16    | 3        |  29.446 μs | 0.0135 μs | 0.0126 μs |  0.95 |
| Encode | Pointers   | S16    | 3        |  42.030 μs | 0.0136 μs | 0.0114 μs |  1.36 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **4**        |  **46.977 μs** | **0.0201 μs** | **0.0188 μs** |  **1.00** |
| Encode | Registers  | S16    | 4        |  55.797 μs | 0.0195 μs | 0.0183 μs |  1.19 |
| Decode | Structures | S16    | 4        |  45.508 μs | 0.1056 μs | 0.0988 μs |  0.97 |
| Encode | Structures | S16    | 4        |  55.837 μs | 0.0217 μs | 0.0192 μs |  1.19 |
| Decode | Library    | S16    | 4        |  47.014 μs | 0.0179 μs | 0.0168 μs |  1.00 |
| Encode | Library    | S16    | 4        |  55.727 μs | 0.0157 μs | 0.0147 μs |  1.19 |
| Decode | Pointers   | S16    | 4        |  43.013 μs | 0.0385 μs | 0.0360 μs |  0.91 |
| Encode | Pointers   | S16    | 4        |  60.371 μs | 0.0197 μs | 0.0175 μs |  1.28 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **6**        |  **76.755 μs** | **0.1391 μs** | **0.1301 μs** |  **1.09** |
| Encode | Registers  | S16    | 6        |  89.502 μs | 0.0249 μs | 0.0233 μs |  1.27 |
| Decode | Structures | S16    | 6        |  70.617 μs | 0.0469 μs | 0.0416 μs |  1.00 |
| Encode | Structures | S16    | 6        |  84.208 μs | 0.0272 μs | 0.0255 μs |  1.19 |
| Decode | Library    | S16    | 6        |  70.488 μs | 0.0678 μs | 0.0634 μs |  1.00 |
| Encode | Library    | S16    | 6        |  84.106 μs | 0.0216 μs | 0.0192 μs |  1.19 |
| Decode | Pointers   | S16    | 6        |  66.967 μs | 0.1583 μs | 0.1480 μs |  0.95 |
| Encode | Pointers   | S16    | 6        |  84.984 μs | 0.0291 μs | 0.0272 μs |  1.21 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S16**    | **8**        | **102.295 μs** | **0.1573 μs** | **0.1472 μs** |  **1.00** |
| Encode | Registers  | S16    | 8        | 119.163 μs | 0.0726 μs | 0.0679 μs |  1.17 |
| Decode | Structures | S16    | 8        | 100.832 μs | 0.1146 μs | 0.1072 μs |  0.99 |
| Encode | Structures | S16    | 8        | 118.234 μs | 0.0503 μs | 0.0470 μs |  1.16 |
| Decode | Library    | S16    | 8        | 101.999 μs | 0.3408 μs | 0.3188 μs |  1.00 |
| Encode | Library    | S16    | 8        | 119.156 μs | 0.0515 μs | 0.0430 μs |  1.17 |
| Decode | Pointers   | S16    | 8        |  95.363 μs | 0.1247 μs | 0.0973 μs |  0.93 |
| Encode | Pointers   | S16    | 8        | 122.372 μs | 0.0663 μs | 0.0620 μs |  1.20 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **1**        |  **14.314 μs** | **0.0049 μs** | **0.0046 μs** |  **1.16** |
| Encode | Registers  | S24    | 1        |  62.707 μs | 0.1780 μs | 0.1665 μs |  5.06 |
| Decode | Structures | S24    | 1        |  12.417 μs | 0.0445 μs | 0.0416 μs |  1.00 |
| Encode | Structures | S24    | 1        |  62.527 μs | 0.0681 μs | 0.0603 μs |  5.05 |
| Decode | Library    | S24    | 1        |  12.393 μs | 0.0043 μs | 0.0038 μs |  1.00 |
| Encode | Library    | S24    | 1        |  62.502 μs | 0.0689 μs | 0.0645 μs |  5.04 |
| Decode | Pointers   | S24    | 1        |  12.701 μs | 0.0044 μs | 0.0039 μs |  1.02 |
| Encode | Pointers   | S24    | 1        | 108.262 μs | 0.1002 μs | 0.0937 μs |  8.74 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **2**        |  **29.633 μs** | **0.0571 μs** | **0.0534 μs** |  **1.13** |
| Encode | Registers  | S24    | 2        | 125.044 μs | 0.1567 μs | 0.1466 μs |  4.75 |
| Decode | Structures | S24    | 2        |  26.345 μs | 0.0131 μs | 0.0123 μs |  1.00 |
| Encode | Structures | S24    | 2        | 125.091 μs | 0.1818 μs | 0.1700 μs |  4.75 |
| Decode | Library    | S24    | 2        |  26.334 μs | 0.0080 μs | 0.0071 μs |  1.00 |
| Encode | Library    | S24    | 2        | 125.865 μs | 0.4079 μs | 0.3816 μs |  4.78 |
| Decode | Pointers   | S24    | 2        |  25.564 μs | 0.0129 μs | 0.0107 μs |  0.97 |
| Encode | Pointers   | S24    | 2        | 188.747 μs | 0.0784 μs | 0.0655 μs |  7.17 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **3**        | **112.360 μs** | **0.2096 μs** | **0.1960 μs** |  **1.00** |
| Encode | Registers  | S24    | 3        | 187.462 μs | 0.2651 μs | 0.2479 μs |  1.67 |
| Decode | Structures | S24    | 3        | 111.863 μs | 0.0377 μs | 0.0334 μs |  1.00 |
| Encode | Structures | S24    | 3        | 187.617 μs | 0.2539 μs | 0.2375 μs |  1.67 |
| Decode | Library    | S24    | 3        | 112.117 μs | 0.0991 μs | 0.0828 μs |  1.00 |
| Encode | Library    | S24    | 3        | 189.675 μs | 0.4902 μs | 0.4586 μs |  1.69 |
| Decode | Pointers   | S24    | 3        | 119.480 μs | 0.0345 μs | 0.0323 μs |  1.07 |
| Encode | Pointers   | S24    | 3        | 264.032 μs | 0.1090 μs | 0.0966 μs |  2.35 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **4**        | **149.596 μs** | **0.1121 μs** | **0.1049 μs** |  **0.99** |
| Encode | Registers  | S24    | 4        | 250.836 μs | 0.4099 μs | 0.3834 μs |  1.67 |
| Decode | Structures | S24    | 4        | 149.419 μs | 0.0921 μs | 0.0769 μs |  0.99 |
| Encode | Structures | S24    | 4        | 250.858 μs | 0.3067 μs | 0.2869 μs |  1.67 |
| Decode | Library    | S24    | 4        | 150.557 μs | 0.1963 μs | 0.1836 μs |  1.00 |
| Encode | Library    | S24    | 4        | 252.160 μs | 0.6152 μs | 0.5754 μs |  1.67 |
| Decode | Pointers   | S24    | 4        | 153.808 μs | 0.0712 μs | 0.0666 μs |  1.02 |
| Encode | Pointers   | S24    | 4        | 336.662 μs | 0.2295 μs | 0.1916 μs |  2.24 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **6**        | **227.571 μs** | **0.3255 μs** | **0.3045 μs** |  **1.00** |
| Encode | Registers  | S24    | 6        | 378.078 μs | 0.4874 μs | 0.4559 μs |  1.65 |
| Decode | Structures | S24    | 6        | 229.730 μs | 0.2238 μs | 0.2093 μs |  1.01 |
| Encode | Structures | S24    | 6        | 378.532 μs | 0.4306 μs | 0.3817 μs |  1.66 |
| Decode | Library    | S24    | 6        | 228.513 μs | 0.5272 μs | 0.4932 μs |  1.00 |
| Encode | Library    | S24    | 6        | 379.230 μs | 0.5842 μs | 0.4879 μs |  1.66 |
| Decode | Pointers   | S24    | 6        | 222.823 μs | 0.2172 μs | 0.1926 μs |  0.98 |
| Encode | Pointers   | S24    | 6        | 489.441 μs | 0.3445 μs | 0.3054 μs |  2.14 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S24**    | **8**        | **306.570 μs** | **0.1431 μs** | **0.1269 μs** |  **0.99** |
| Encode | Registers  | S24    | 8        | 505.330 μs | 0.5553 μs | 0.5195 μs |  1.63 |
| Decode | Structures | S24    | 8        | 310.351 μs | 0.3719 μs | 0.3479 μs |  1.00 |
| Encode | Structures | S24    | 8        | 504.007 μs | 0.4764 μs | 0.4457 μs |  1.62 |
| Decode | Library    | S24    | 8        | 310.445 μs | 0.3357 μs | 0.3140 μs |  1.00 |
| Encode | Library    | S24    | 8        | 506.831 μs | 1.0867 μs | 1.0165 μs |  1.63 |
| Decode | Pointers   | S24    | 8        | 291.258 μs | 0.2577 μs | 0.2411 μs |  0.94 |
| Encode | Pointers   | S24    | 8        | 646.786 μs | 0.7802 μs | 0.7298 μs |  2.08 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **1**        |   **8.239 μs** | **0.0168 μs** | **0.0149 μs** |  **1.00** |
| Encode | Registers  | S32    | 1        |  10.309 μs | 0.0036 μs | 0.0032 μs |  1.25 |
| Decode | Structures | S32    | 1        |   8.239 μs | 0.0078 μs | 0.0065 μs |  1.00 |
| Encode | Structures | S32    | 1        |  10.303 μs | 0.0154 μs | 0.0144 μs |  1.25 |
| Decode | Library    | S32    | 1        |   8.220 μs | 0.0113 μs | 0.0100 μs |  1.00 |
| Encode | Library    | S32    | 1        |  10.337 μs | 0.0036 μs | 0.0034 μs |  1.26 |
| Decode | Pointers   | S32    | 1        |   8.051 μs | 0.0015 μs | 0.0014 μs |  0.98 |
| Encode | Pointers   | S32    | 1        |  10.274 μs | 0.0022 μs | 0.0021 μs |  1.25 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **2**        |  **18.317 μs** | **0.0089 μs** | **0.0079 μs** |  **0.95** |
| Encode | Registers  | S32    | 2        |  23.826 μs | 0.0083 μs | 0.0077 μs |  1.24 |
| Decode | Structures | S32    | 2        |  19.247 μs | 0.0297 μs | 0.0263 μs |  1.00 |
| Encode | Structures | S32    | 2        |  23.841 μs | 0.0103 μs | 0.0091 μs |  1.24 |
| Decode | Library    | S32    | 2        |  19.221 μs | 0.0133 μs | 0.0124 μs |  1.00 |
| Encode | Library    | S32    | 2        |  23.782 μs | 0.0082 μs | 0.0073 μs |  1.24 |
| Decode | Pointers   | S32    | 2        |  17.985 μs | 0.0308 μs | 0.0288 μs |  0.94 |
| Encode | Pointers   | S32    | 2        |  23.696 μs | 0.0105 μs | 0.0098 μs |  1.23 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **3**        |  **32.112 μs** | **0.0534 μs** | **0.0499 μs** |  **1.21** |
| Encode | Registers  | S32    | 3        |  42.492 μs | 0.0403 μs | 0.0358 μs |  1.60 |
| Decode | Structures | S32    | 3        |  26.478 μs | 0.0285 μs | 0.0267 μs |  1.00 |
| Encode | Structures | S32    | 3        |  37.378 μs | 0.0169 μs | 0.0158 μs |  1.41 |
| Decode | Library    | S32    | 3        |  26.545 μs | 0.0310 μs | 0.0275 μs |  1.00 |
| Encode | Library    | S32    | 3        |  37.233 μs | 0.0302 μs | 0.0283 μs |  1.40 |
| Decode | Pointers   | S32    | 3        |  27.032 μs | 0.0761 μs | 0.0712 μs |  1.02 |
| Encode | Pointers   | S32    | 3        |  37.324 μs | 0.0211 μs | 0.0187 μs |  1.41 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **4**        |  **43.587 μs** | **0.1970 μs** | **0.1843 μs** |  **1.00** |
| Encode | Registers  | S32    | 4        |  57.222 μs | 0.0421 μs | 0.0394 μs |  1.31 |
| Decode | Structures | S32    | 4        |  44.029 μs | 0.1692 μs | 0.1500 μs |  1.01 |
| Encode | Structures | S32    | 4        |  56.829 μs | 0.0312 μs | 0.0292 μs |  1.30 |
| Decode | Library    | S32    | 4        |  43.733 μs | 0.1054 μs | 0.0986 μs |  1.00 |
| Encode | Library    | S32    | 4        |  57.112 μs | 0.0219 μs | 0.0205 μs |  1.31 |
| Decode | Pointers   | S32    | 4        |  44.453 μs | 0.2270 μs | 0.2124 μs |  1.02 |
| Encode | Pointers   | S32    | 4        |  66.063 μs | 0.0315 μs | 0.0295 μs |  1.51 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **6**        |  **78.902 μs** | **0.1671 μs** | **0.1563 μs** |  **1.17** |
| Encode | Registers  | S32    | 6        |  95.167 μs | 0.0736 μs | 0.0689 μs |  1.41 |
| Decode | Structures | S32    | 6        |  68.000 μs | 0.1015 μs | 0.0950 μs |  1.01 |
| Encode | Structures | S32    | 6        |  82.538 μs | 0.0531 μs | 0.0444 μs |  1.22 |
| Decode | Library    | S32    | 6        |  67.559 μs | 0.1346 μs | 0.1259 μs |  1.00 |
| Encode | Library    | S32    | 6        |  82.164 μs | 0.1063 μs | 0.0994 μs |  1.22 |
| Decode | Pointers   | S32    | 6        |  68.548 μs | 0.3843 μs | 0.3595 μs |  1.01 |
| Encode | Pointers   | S32    | 6        | 100.199 μs | 0.1051 μs | 0.0932 μs |  1.48 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **S32**    | **8**        | **107.241 μs** | **0.0907 μs** | **0.0849 μs** |  **1.00** |
| Encode | Registers  | S32    | 8        | 127.533 μs | 0.1385 μs | 0.1296 μs |  1.19 |
| Decode | Structures | S32    | 8        | 106.846 μs | 0.1155 μs | 0.1081 μs |  1.00 |
| Encode | Structures | S32    | 8        | 105.118 μs | 0.0815 μs | 0.0763 μs |  0.98 |
| Decode | Library    | S32    | 8        | 106.997 μs | 0.0756 μs | 0.0707 μs |  1.00 |
| Encode | Library    | S32    | 8        | 128.080 μs | 0.1033 μs | 0.0863 μs |  1.20 |
| Decode | Pointers   | S32    | 8        | 105.294 μs | 0.0819 μs | 0.0766 μs |  0.98 |
| Encode | Pointers   | S32    | 8        | 126.450 μs | 0.1274 μs | 0.1192 μs |  1.18 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **1**        |  **12.162 μs** | **0.0040 μs** | **0.0036 μs** |  **1.00** |
| Decode | Structures | U8     | 1        |  12.171 μs | 0.0078 μs | 0.0073 μs |  1.00 |
| Decode | Library    | U8     | 1        |  12.165 μs | 0.0080 μs | 0.0075 μs |  1.00 |
| Decode | Pointers   | U8     | 1        |  11.732 μs | 0.0047 μs | 0.0041 μs |  0.96 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **2**        |  **26.631 μs** | **0.0062 μs** | **0.0058 μs** |  **1.00** |
| Decode | Structures | U8     | 2        |  26.464 μs | 0.0076 μs | 0.0063 μs |  0.99 |
| Decode | Library    | U8     | 2        |  26.644 μs | 0.0064 μs | 0.0057 μs |  1.00 |
| Decode | Pointers   | U8     | 2        |  24.483 μs | 0.0083 μs | 0.0078 μs |  0.92 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **3**        |  **41.649 μs** | **0.0102 μs** | **0.0090 μs** |  **1.05** |
| Decode | Structures | U8     | 3        |  39.563 μs | 0.0062 μs | 0.0055 μs |  1.00 |
| Decode | Library    | U8     | 3        |  39.574 μs | 0.0070 μs | 0.0066 μs |  1.00 |
| Decode | Pointers   | U8     | 3        |  36.489 μs | 0.0072 μs | 0.0068 μs |  0.92 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **4**        |  **55.004 μs** | **0.0664 μs** | **0.0621 μs** |  **1.00** |
| Decode | Structures | U8     | 4        |  54.349 μs | 0.0244 μs | 0.0228 μs |  0.99 |
| Decode | Library    | U8     | 4        |  55.043 μs | 0.0563 μs | 0.0499 μs |  1.00 |
| Decode | Pointers   | U8     | 4        |  51.230 μs | 0.0096 μs | 0.0080 μs |  0.93 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **6**        |  **85.191 μs** | **0.0227 μs** | **0.0213 μs** |  **1.03** |
| Decode | Structures | U8     | 6        |  82.755 μs | 0.0367 μs | 0.0344 μs |  1.00 |
| Decode | Library    | U8     | 6        |  82.719 μs | 0.0324 μs | 0.0303 μs |  1.00 |
| Decode | Pointers   | U8     | 6        |  74.441 μs | 0.0497 μs | 0.0465 μs |  0.90 |
|        |            |        |          |            |           |           |       |
| **Decode** | **Registers**  | **U8**     | **8**        | **114.702 μs** | **0.0506 μs** | **0.0473 μs** |  **1.00** |
| Decode | Structures | U8     | 8        | 112.586 μs | 0.0612 μs | 0.0511 μs |  0.98 |
| Decode | Library    | U8     | 8        | 114.554 μs | 0.0435 μs | 0.0407 μs |  1.00 |
| Decode | Pointers   | U8     | 8        | 104.663 μs | 0.0405 μs | 0.0359 μs |  0.91 |
