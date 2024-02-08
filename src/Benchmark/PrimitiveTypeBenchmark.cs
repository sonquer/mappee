using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Mappe;
using Nelibur.ObjectMapper;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [SimpleJob(RuntimeMoniker.Net462)]
    [RPlotExporter]
    public class PrimitiveTypeBenchmark
    {
        private readonly SourceWithPrimitiveTypes _source = CreateSource();

        public PrimitiveTypeBenchmark()
        {
            InitTinyMapper();
            InitMappe();
        }

        private void InitTinyMapper()
        {
            TinyMapper.Bind<SourceWithPrimitiveTypes, TargetWithPrimitiveTypes>();
        }

        private void InitMappe()
        {
            Mapper.Bind<SourceWithPrimitiveTypes, TargetWithPrimitiveTypes>()
                .Compile();
        }

        private static SourceWithPrimitiveTypes CreateSource()
        {
            return new SourceWithPrimitiveTypes
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

        [Benchmark]
        public void PrimitiveType_TinyMapper()
        {
            var result = TinyMapper.Map<SourceWithPrimitiveTypes, TargetWithPrimitiveTypes>(_source);
        }

        [Benchmark]
        public void PrimitiveType_Mappe()
        {
            var result = Mappe.Mapper.Map<SourceWithPrimitiveTypes, TargetWithPrimitiveTypes>(_source);
        }

        [Benchmark]
        public void PrimitiveType_Handwritten()
        {
            var result = new SourceWithPrimitiveTypes()
            {
                Bool = _source.Bool,
                Byte = _source.Byte,
                Char = _source.Char,
                DateTime = _source.DateTime,
                Decimal = _source.Decimal,
                Email = _source.Email,
                FirstName = _source.FirstName,
                Float = _source.Float,
                Int = _source.Int,
                LastName = _source.LastName,
                Long = _source.Long,
                Nickname = _source.Nickname,
                Short = _source.Short
            };
        }
    }


    public sealed class SourceWithPrimitiveTypes
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


    public sealed class TargetWithPrimitiveTypes
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
