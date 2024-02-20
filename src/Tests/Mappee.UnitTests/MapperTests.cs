using Mappee.UnitTests.Models;

namespace Mappee.UnitTests
{
    public class MapperTests
    {
        public MapperTests()
        {
            Mapper.Bind<TestObject, TestObjectDto>()
                .Bind<TestObjectField, TestObjectFieldDto>()
                .IgnoreMember<TestObject, TestObjectDto>(source => source.Id)
                .ForMember<TestObject, TestObjectDto>("TEST_VALUE", destination => destination.ConstValue)
                .AfterMap<TestObject, TestObjectDto>((_, destination) =>
                {
                    destination.Char = destination.FirstName.First();
                })
                .BeforeMap<TestObject, TestObjectDto>((source, _) =>
                {
                    source.Nickname = $"{source.FirstName}{source.LastName}".ToLower();
                })
                .Compile();
        }

        [Fact]
        public void Map_BindsAndMapsFieldsCorrectly_ToDtoType()
        {
            var source = new TestObjectField
            {
                Id = 1,
                Name = "Test",
            };

            var result = Mapper.Map<TestObjectFieldDto>(source);

            Assert.Equal(source.Name, result.Name);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsFieldsCorrectly_ToOriginalAndDtoType()
        {
            var source = new TestObjectField
            {
                Id = 1,
                Name = "Test",
            };

            var result = Mapper.Map<TestObjectField, TestObjectFieldDto>(source);

            Assert.Equal(source.Name, result.Name);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsObjectsCorrectly_WithConstValue()
        {
            var source = new TestObject
            {
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var result = Mapper.Map<TestObjectDto>(source);

            Assert.Equal(source.FirstName, result.FirstName);
            Assert.Equal(source.LastName, result.LastName);
            Assert.Equal("TEST_VALUE", result.ConstValue);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsObjectsCorrectly_WithIgnoreMember()
        {
            var source = new TestObject
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var result = Mapper.Map<TestObjectDto>(source);

            Assert.Equal(0, result.Id);
            Assert.Equal(source.FirstName, result.FirstName);
            Assert.Equal(source.LastName, result.LastName);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsObjectsCorrectly_WithBeforeMap()
        {
            var source = new TestObject
            {
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var result = Mapper.Map<TestObjectDto>(source);

            Assert.Equal(source.FirstName, result.FirstName);
            Assert.Equal($"{source.FirstName}{source.LastName}".ToLower(), result.Nickname);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsObjectsCorrectly_WithAfterMap()
        {
            var source = new TestObject
            {
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var result = Mapper.Map<TestObjectDto>(source);

            Assert.Equal(source.FirstName, result.FirstName);
            Assert.Equal('J', result.Char);
            Assert.NotSame(source, result);
        }
    }
}
