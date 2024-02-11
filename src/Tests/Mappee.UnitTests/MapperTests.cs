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
    }
}
