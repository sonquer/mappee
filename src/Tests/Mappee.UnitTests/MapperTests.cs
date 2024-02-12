using Mappee.UnitTests.Models;

namespace Mappee.UnitTests
{
    public class MapperTests
    {
        [Fact]
        public void Map_BindsAndMapsFieldsCorrectly_ToDtoType()
        {
            Mapper.Bind<TestObjectField, TestObjectFieldDto>()
                .Compile();

            var source = new TestObjectField
            {
                Id = 1,
                Name = "Test",
            };

            var result = Mapper.Map<TestObjectFieldDto>(source);

            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Name, result.Name);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsFieldsCorrectly_ToOriginalAndDtoType()
        {
            Mapper.Bind<TestObjectField, TestObjectFieldDto>()
                .Compile();

            var source = new TestObjectField
            {
                Id = 1,
                Name = "Test",
            };

            var result = Mapper.Map<TestObjectField, TestObjectFieldDto>(source);

            Assert.Equal(source.Id, result.Id);
            Assert.Equal(source.Name, result.Name);
            Assert.NotSame(source, result);
        }

        [Fact]
        public void Map_BindsAndMapsObjectsCorrectly_WithConstValue()
        {
            Mapper.Bind<TestObject, TestObjectDto>()
                .ForMember<TestObject, TestObjectDto>("TEST_VALUE", destination => destination.ConstValue)
                .Bind<TestObjectField, TestObjectFieldDto>()
                .Compile();

            var source = new TestObject
            {
                Id = 1
            };

            var result = Mapper.Map<TestObjectDto>(source);

            Assert.Equal(source.Id, result.Id);
            Assert.Equal("TEST_VALUE", result.ConstValue);
            Assert.NotSame(source, result);
        }
    }
}
