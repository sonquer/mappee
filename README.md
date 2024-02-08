Mappee - Fastest avaiable mapper for .NET
======================================================
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/sonquer/mappee/blob/main/LICENSE)
[![First Timers](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](http://www.firsttimersonly.com/)

Mappee is a high performance object-to-object mapper for .NET. It is a simple and easy to use library that can be used to map objects of different types. It is designed to be fast and efficient, and it is the fastest available mapper for .NET.
The main idea is to map objects using precompiled code, which makes it much faster than other mappers that use reflection.
Generated code is cached, so it is only generated once for each type pair, and then reused for all subsequent mappings.

## Features
- Fast and efficient
- Simple and easy to use

## Mappee vs other mappers (performance comparison)
#### 2500 items in 3 different collections and 2500 nested objects (like linked list)

![Performance Comparison 2500](/assets/images/2500-barplot.png)

#### 1,5,20 items in 3 different collections and 1,5,20 nested objects (like linked list)

![Performance Comparison 1,5,20](/assets/images/5-10-20-barplot.png)

## Supported platforms
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET Standard 2.1
- .NET Standard 2.0
- .NET Framework 4.8.1