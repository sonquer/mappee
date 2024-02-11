using Benchmark.Models.DataTransferObjects;
using Benchmark.Models.Entities;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mappee;
using System;
using System.Collections.Generic;
using AutoMapperForBenchmark = AutoMapper;
using TinyMapperForBenchmark = Nelibur.ObjectMapper;

namespace Benchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [SimpleJob(RuntimeMoniker.Net80, baseline: true)]
    public class MappersBenchmark
    {
        private TestObject _source;

        [Params(2500)]
        public int Items;

        [GlobalSetup]
        public void Setup()
        {
            _source = new TestObject
            {
                Id = Items,
                FirstName = "Jan",
                LastName = "Kowalski",
                Nickname = "jankowalski",
                Email = "example@example.com",
                Short = short.MaxValue,
                Long = long.MaxValue,
                Int = int.MaxValue,
                Float = float.MaxValue,
                Decimal = decimal.MaxValue,
                DateTime = DateTime.MaxValue,
                Char = 'a',
                Bool = true,
                Byte = byte.MaxValue
            };

            for (var i = 0; i < Items; i++)
            {
                ((List<TestObjectModification>)_source.Modifications).Add(new TestObjectModification
                {
                    Id = i,
                    Type = $"Type {i}",
                    Date = DateTime.Now,
                    Author = $"Author {i}"
                });

                _source.Fields.Add(new TestObjectField
                {
                    Id = i,
                    Name = $"Name {i}",
                });

                _source.Links.Add(new TestObjectLink
                {
                    Id = i,
                    Url = $"Url {i}"
                });
            }

            var child = _source;
            for (var j = 0; j < Items; j++)
            {
                child.Child = CreateSource(j);
                child = child.Child;
            }

            TinyMapperForBenchmark.TinyMapper.Bind<TestObject, TestObjectDto>();
            TinyMapperForBenchmark.TinyMapper.Bind<TestObjectModification, TestObjectModificationDto>();
            TinyMapperForBenchmark.TinyMapper.Bind<TestObjectField, TestObjectFieldDto>();
            TinyMapperForBenchmark.TinyMapper.Bind<TestObjectLink, TestObjectLinkDto>();

            Mapper.Bind<TestObject, TestObjectDto>()
                .Bind<TestObjectModification, TestObjectModificationDto>()
                .Bind<TestObjectField, TestObjectFieldDto>()
                .Bind<TestObjectLink, TestObjectLinkDto>()
                .Compile();

            AutoMapperForBenchmark.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<TestObjectModification, TestObjectModificationDto>();
                cfg.CreateMap<TestObjectField, TestObjectFieldDto>();
                cfg.CreateMap<TestObjectLink, TestObjectLinkDto>();
                cfg.CreateMap<TestObject, TestObjectDto>();
            });
        }

        [Benchmark]
        public void AutoMapper()
        {
            var _ = AutoMapperForBenchmark.Mapper.Map<TestObjectDto>(_source);
        }

        [Benchmark]
        public void TinyMapper()
        {
            var _ = TinyMapperForBenchmark.TinyMapper.Map<TestObject, TestObjectDto>(_source);
        }

        [Benchmark]
        public void Mappee()
        {
            var _ = Mapper.Map<TestObject, TestObjectDto>(_source);
        }

        [Benchmark]
        public void Handwritten()
        {
            var _ = CustomMapping(_source);
        }

        private TestObjectDto CustomMapping(TestObject source)
        {
            var item = new TestObjectDto();
            item.Child = source.Child != null ? CustomMapping(source.Child) : null;
            ((List<TestObjectLink>)source.Links)?.ForEach(x => item.Links.Add(new TestObjectLinkDto { Id = x.Id, Url = x.Url }));
            ((List<TestObjectModification>)source.Modifications)?.ForEach(x => item.Modifications.Add(new TestObjectModificationDto { Id = x.Id, Type = x.Type, Date = x.Date, Author = x.Author }));
            source.Fields?.ForEach(x => item.Fields.Add(new TestObjectFieldDto { Id = x.Id, Name = x.Name }));
            item.Short = source.Short;
            item.Nickname = source.Nickname;
            item.Long = source.Long;
            item.LastName = source.LastName;
            item.Int = source.Int;
            item.Float = source.Float;
            item.FirstName = source.FirstName;
            item.Email = source.Email;
            item.Decimal = source.Decimal;
            item.DateTime = source.DateTime;
            item.Char = source.Char;
            item.Byte = source.Byte;
            item.Bool = source.Bool;
            item.Id = source.Id;
            return item;
        }

        private TestObject CreateSource(int n)
        {
            return new TestObject
            {
                Id = n,
                FirstName = "Jan",
                LastName = "Kowalski",
                Nickname = $"jankowalski{n}",
                Email = "example@example.com",
                Short = short.MaxValue,
                Long = long.MaxValue,
                Int = int.MaxValue,
                Float = float.MaxValue,
                Decimal = decimal.MaxValue,
                DateTime = DateTime.MaxValue,
                Char = 'a',
                Bool = true,
                Byte = byte.MaxValue
            };
        }
    }
}
