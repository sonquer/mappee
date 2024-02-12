Mappee - Fastest avaiable mapper for .NET
======================================================
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/sonquer/mappee/blob/main/LICENSE)
[![First Timers](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](http://www.firsttimersonly.com/)
[![Nuget](https://img.shields.io/nuget/v/mappee.svg)](https://www.nuget.org/packages/Mappee/)

Mappee is a high performance object-to-object mapper for .NET. It is a simple and easy to use library that can be used to map objects of different types. It is designed to be fast and efficient, and it is the fastest available mapper for .NET.
The main idea is to map objects using precompiled code, which makes it much faster than other mappers that use reflection.
Generated code is cached, so it is only generated once for each type pair, and then reused for all subsequent mappings.

## Features
- Fast and efficient
- Simple and easy to use

## Mappee vs other mappers (performance comparison)

### Benchmarks

#### 2500 items in 3 different collections and 2500 nested objects (like linked list)

![Performance Comparison 2500](https://raw.githubusercontent.com/sonquer/mappee/main/assets/images/2500-barplot.png)

##### 1,5,20 items in 3 different collections and 1,5,20 nested objects (like linked list)

![Performance Comparison 1,5,20](https://raw.githubusercontent.com/sonquer/mappee/main/assets/images/5-10-20-barplot.png)

## Getting Started

### Using static methods
```csharp
Mapper.Bind<TestObject, TestObjectDto>()
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
    profile.Bind<TestObject, TestObjectDto>();
    //invoke compile method is not necessary, it will be called automatically
});

//[...]

var serviceProvider = servicesCollection.BuildServiceProvider();
var scope = serviceProvider.CreateScope();

var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

var testObject = new TestObject();
var testObjectDto = mapper.Map<TestObject, TestObjectDto>(testObject);
```

## Supported platforms
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET Standard 2.1
- .NET Standard 2.0
- .NET Framework 4.8.1
