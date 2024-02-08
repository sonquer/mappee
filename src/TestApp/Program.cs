using Mappe;
using Microsoft.Extensions.DependencyInjection;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using TestApp.Models.DataTransferObjects;
using TestApp.Models.Entities;

namespace TestApp
{
    internal class Program
    {
        private static void Main()
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddMappe();

            TinyMapper.Bind<TestObject, TestObjectDto>();
            TinyMapper.Bind<TestObjectModification, TestObjectModificationDto>();
            TinyMapper.Bind<TestObjectField, TestObjectFieldDto>();
            TinyMapper.Bind<TestObjectLink, TestObjectLinkDto>();

            Mapper.Bind<TestObject, TestObjectDto>()
                .Bind<TestObjectModification, TestObjectModificationDto>()
                .Bind<TestObjectField, TestObjectFieldDto>()
                .Bind<TestObjectLink, TestObjectLinkDto>()
                .Compile();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<TestObjectModification, TestObjectModificationDto>();
                cfg.CreateMap<TestObjectField, TestObjectFieldDto>();
                cfg.CreateMap<TestObjectLink, TestObjectLinkDto>();
                cfg.CreateMap<TestObject, TestObjectDto>();
            });

            var serviceProvider = servicesCollection.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();

            var mappe = scope.ServiceProvider.GetRequiredService<IMapper>();

            var n = 2_500;
            var source = new TestObject
            {
                Id = n,
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

            for (var i = 0; i < n; i++)
            {
                source.Modifications.Add(new TestObjectModification
                {
                    Id = i,
                    Type = $"Type {i}",
                    Date = DateTime.Now,
                    Author = $"Author {i}"
                });

                source.Fields.Add(new TestObjectField
                {
                    Id = i,
                    Name = $"Name {i}",
                });

                source.Links.Add(new TestObjectLink
                {
                    Id = i,
                    Url = $"Url {i}"
                });
            }

            var child = source;
            for (var j = 0; j < n; j++)
            {
                child.Child = CreateSource(j);
                child = child.Child;
            }

            var sw = new System.Diagnostics.Stopwatch();
            //var destination = mappe.Map<TestObject, TestObjectDto>(source);
            var destination = AutoMapper.Mapper.Map<TestObject, TestObjectDto>(source);
            //var destination = TinyMapper.Map<TestObjectDto>(source);
            sw.Stop();

            //Console.WriteLine($"Source = {JsonConvert.SerializeObject(source, Formatting.Indented)}");
            //Console.WriteLine();
            //Console.WriteLine($"Destination = {JsonConvert.SerializeObject(destination, Formatting.Indented)}");

            Console.WriteLine();
            Console.WriteLine($"Elapsed = {sw.ElapsedMilliseconds}");
        }

        private static TestObject CreateSource(int n)
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