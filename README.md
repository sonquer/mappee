Mappee - Fastest avaiable mapper for .NET
======================================================
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg?colorA=192330&colorB=c70039&style=for-the-badge)](https://github.com/sonquer/mappee/blob/main/LICENSE)
[![First Timers](https://img.shields.io/badge/first--timers--only-friendly-blue.svg?colorA=192330&style=for-the-badge)](http://www.firsttimersonly.com/)
[![CodeFactor](https://img.shields.io/codefactor/grade/github/sonquer/mappee?colorA=192330&style=for-the-badge)](https://www.codefactor.io/repository/github/sonquer/mappee)
[![Nuget](https://img.shields.io/nuget/v/mappee.svg?colorA=192330&style=for-the-badge)](https://www.nuget.org/packages/Mappee/)


Mappee is a high performance object-to-object mapper for .NET. It is a simple and easy to use library that can be used to map objects of different types. It is designed to be fast and efficient, and it is the fastest available mapper for .NET.
The main idea is to map objects using precompiled code, which makes it much faster than other mappers that use reflection.
Generated code is cached, so it is only generated once for each type pair, and then reused for all subsequent mappings.

## Features
- Fast and efficient
- Simple and easy to use

## Getting Started

### Using static methods
```csharp
Mapper.Bind<TestObject, TestObjectDto>()
    .IgnoreMember<TestObject, TestObjectDto>(e => e.Name);
    .BeforeMap<TestObject, TestObjectDto>((source, _) =>
    {
        source.Nickname = $"{source.FirstName}{source.LastName}".ToLower();
    })
    .AfterMap<TestObject, TestObjectDto>((_, destination) =>
    {
        destination.Char = destination.FirstName.First();
    })
    .Bind<TestObjectModification, TestObjectModificationDto>()
    .Bind<TestObjectField, TestObjectFieldDto>()
    .Bind<TestObjectLink, TestObjectLinkDto>()
    .Compile();

var testObject = new TestObject();

var testObjectDto = Mapper.Map<TestObjectDto>(testObject);
```

### Using dependency injection
```csharp
var servicesCollection = new ServiceCollection();
servicesCollection.AddMappee(profile =>
{
    profile.Bind<TestObject, TestObjectDto>()
        .IgnoreMember<TestObject, TestObjectDto>(e => e.Name)
        .BeforeMap<TestObject, TestObjectDto>((source, _) =>
        {
            source.Nickname = $"{source.FirstName}{source.LastName}".ToLower();
        })
        .AfterMap<TestObject, TestObjectDto>((_, destination) =>
        {
            destination.Char = destination.FirstName.First();
        });

    //invoke compile method is not necessary, it will be called automatically
});

//[...]

var serviceProvider = servicesCollection.BuildServiceProvider();
var scope = serviceProvider.CreateScope();

var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

var testObject = new TestObject();
var testObjectDto = mapper.Map<TestObject, TestObjectDto>(testObject);
```

## Mappee vs other mappers (performance comparison)

### Benchmarks

#### 2500 items in 3 different collections and 2500 nested objects (like linked list)

![Performance Comparison 2500](https://raw.githubusercontent.com/sonquer/mappee/main/assets/images/2500-barplot.png)

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
12th Gen Intel Core i7-12700K, 1 CPU, 20 logical and 12 physical cores
.NET SDK 8.0.101
  [Host]   : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  
```
| Method      | Items | Mean       | Error    | StdDev   | Ratio | Rank | Gen0     | Gen1     | Gen2    | Allocated | Alloc Ratio |
|------------ |------ |-----------:|---------:|---------:|------:|-----:|---------:|---------:|--------:|----------:|------------:|
| AutoMapper  | 2500  | 1,697.0 μs | 33.92 μs | 58.50 μs |  1.00 |    1 | 144.5313 | 142.5781 | 35.1563 |    1.5 MB |        1.00 |
| TinyMapper  | 2500  |   426.8 μs |  1.94 μs |  1.82 μs |  1.00 |    1 |  99.1211 |  49.3164 |       - |   1.24 MB |        1.00 |
| Mappee      | 2500  |   268.0 μs |  2.85 μs |  2.67 μs |  1.00 |    1 | 140.6250 |  93.2617 |       - |   1.75 MB |        1.00 |
| Handwritten | 2500  |   233.5 μs |  4.08 μs |  3.62 μs |  1.00 |    1 | 122.0703 |  66.4063 |       - |   1.52 MB |        1.00 |

---

##### 1,5,20 items in 3 different collections and 1,5,20 nested objects (like linked list)

![Performance Comparison 1,5,20](https://raw.githubusercontent.com/sonquer/mappee/main/assets/images/5-10-20-barplot.png)

```
BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3155/23H2/2023Update/SunValley3)
12th Gen Intel Core i7-12700K, 1 CPU, 20 logical and 12 physical cores
.NET SDK 8.0.101
  [Host]   : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  
```
| Method      | Items | Mean       | Error    | StdDev   | Ratio | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------ |------ |-----------:|---------:|---------:|------:|-----:|-------:|-------:|----------:|------------:|
| AutoMapper  | 5     |   928.6 ns | 12.45 ns | 11.64 ns |  1.00 |    1 | 0.2775 | 0.0029 |   3.55 KB |        1.00 |
| TinyMapper  | 5     |   930.4 ns |  9.43 ns |  8.82 ns |  1.00 |    1 | 0.2356 | 0.0019 |   3.01 KB |        1.00 |
| Mappee      | 5     |   457.6 ns |  9.17 ns | 10.56 ns |  1.00 |    1 | 0.3252 | 0.0029 |   4.16 KB |        1.00 |
| Handwritten | 5     |   368.8 ns |  1.90 ns |  1.58 ns |  1.00 |    1 | 0.2813 | 0.0024 |   3.59 KB |        1.00 |
| AutoMapper  | 10    | 1,663.7 ns | 14.35 ns | 13.42 ns |  1.00 |    1 | 0.5302 | 0.0095 |   6.79 KB |        1.00 |
| TinyMapper  | 10    | 1,708.8 ns | 18.58 ns | 17.38 ns |  1.00 |    1 | 0.4387 | 0.0057 |    5.6 KB |        1.00 |
| Mappee      | 10    |   832.1 ns | 13.57 ns | 12.70 ns |  1.00 |    1 | 0.6113 | 0.0095 |    7.8 KB |        1.00 |
| Handwritten | 10    |   683.4 ns |  5.36 ns |  4.75 ns |  1.00 |    1 | 0.5302 | 0.0076 |   6.77 KB |        1.00 |
| AutoMapper  | 20    | 3,082.1 ns | 13.87 ns | 12.30 ns |  1.00 |    1 | 1.0376 | 0.0343 |  13.26 KB |        1.00 |
| TinyMapper  | 20    | 3,334.9 ns | 65.42 ns | 77.87 ns |  1.00 |    1 | 0.8392 | 0.0229 |  10.72 KB |        1.00 |
| Mappee      | 20    | 1,575.2 ns | 31.47 ns | 52.58 ns |  1.00 |    1 | 1.1768 | 0.0343 |  15.03 KB |        1.00 |
| Handwritten | 20    | 1,313.9 ns | 15.95 ns | 14.14 ns |  1.00 |    1 | 1.0223 | 0.0286 |  13.06 KB |        1.00 |

## Supported platforms
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET Standard 2.1
- .NET Standard 2.0
- .NET Framework 4.8.1