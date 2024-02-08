﻿using BenchmarkDotNet.Attributes;
using Mappe;
using Nelibur.ObjectMapper;

namespace Benchmark
{
    public class CollectionBenchmark
    {
        private const int CollectionLength = 100;

        private readonly SourceWithCollections _source = CreateSource();

        private const int Iterations = 1;

        private readonly IMapper _mappe = new MappeExecutor();

        public CollectionBenchmark()
        {
            InitTinyMapper();
        }

        private void InitTinyMapper()
        {
            TinyMapper.Bind<SourceWithCollections, TargetWithCollections>();
        }

        private void InitMappe()
        {
            Mappe.Mapper.Bind<SourceWithCollections, TargetWithCollections>()
                .Compile();
        }

        [Benchmark]
        public void CollectionMapping_TinyMapper()
        {
            for (int i = 0; i < Iterations; i++)
            {
                TinyMapper.Map<TargetWithCollections>(_source);
            }
        }

        [Benchmark]
        public void CollectionMapping_Mappe()
        {
            for (int i = 0; i < Iterations; i++)
            {
                _mappe.Map<SourceWithCollections, TargetWithCollections>(_source);
            }
        }

        [Benchmark]
        public void CollectionMapping_Handwritten()
        {
            for (int i = 0; i < Iterations; i++)
            {
                HandwrittenMap(_source, new TargetWithCollections());
            }
        }

        private static TargetWithCollections HandwrittenMap(SourceWithCollections source, TargetWithCollections target)
        {
            target.StringList.AddRange(source.StringList);
            source.ItemList.ForEach(x => target.ItemList.Add(HandwrittenMap(x, new Item())));
            return target;
        }

        private static Item HandwrittenMap(Item source, Item target)
        {
            target.Bool = source.Bool;
            target.Byte = source.Byte;
            target.Char = source.Char;
            target.DateTime = source.DateTime;
            target.Decimal = source.Decimal;
            target.Float = source.Float;
            target.Int = source.Int;
            target.Long = source.Long;
            target.Short = source.Short;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Nickname = source.Nickname;
            target.Email = source.Email;
            return target;
        }

        private static SourceWithCollections CreateSource()
        {
            var result = new SourceWithCollections();

            for (int i = 0; i < CollectionLength; i++)
            {
                result.StringList.Add(Guid.NewGuid().ToString());
                result.ItemList.Add(CreateItem());
            }
            return result;
        }

        private static Item CreateItem()
        {
            return new Item
            {
                FirstName = "John",
                LastName = "Doe",
                Nickname = "TinyMapper",
                Email = "support@TinyMapper.net",
                Short = 3,
                Long = 10,
                Int = 5,
                Float = 4.9f,
                Decimal = 4.0m,
                DateTime = DateTime.Now,
                Char = 'a',
                Bool = true,
                Byte = 0
            };
        }
    }


    public class SourceWithCollections
    {
        public SourceWithCollections()
        {
            ItemList = new List<Item>();
            StringList = new List<string>();
        }

        public List<Item> ItemList { get; set; }
        public List<string> StringList { get; set; }
    }


    public class TargetWithCollections
    {
        public TargetWithCollections()
        {
            ItemList = new List<Item>();
            StringList = new List<string>();
        }

        public List<Item> ItemList { get; set; }
        public List<string> StringList { get; set; }
    }


    public class Item
    {
        public bool Bool { get; set; }
        public byte Byte { get; set; }
        public char Char { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Decimal { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }
        public string LastName { get; set; }
        public long Long { get; set; }
        public string Nickname { get; set; }
        public short Short { get; set; }
    }
}
