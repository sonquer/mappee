using Mappe;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace TestApp
{

    public sealed class Source
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


    public sealed class Destination
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

    internal class Program
    {
        private static void Main()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddMappe();

            Mapper.Bind<Source, Destination>()
                .Compile();

            GC.Collect();

            var serviceProvider = servicesCollection.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();

            var mappe = scope.ServiceProvider.GetRequiredService<IMapper>();

            var source = new Source
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

            var destination = mappe.Map<Source, Destination>(source);

            Console.WriteLine($"Source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");
            Console.WriteLine();
            Console.WriteLine($"Destination = {JsonConvert.SerializeObject(destination, Formatting.Indented)}");
        }
    }
}