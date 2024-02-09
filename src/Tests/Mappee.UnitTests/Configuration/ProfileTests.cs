using Mappee.Configuration;
using Mappee.UnitTests.Models;

namespace Mappee.UnitTests.Configuration
{
    public class ProfileTests
    {
        [Fact]
        public void Compile_ReturnsCompilationResult_WhenSuccessful()
        {
            var profile = new Profile();
            profile.Map<TestObject, TestObjectDto>();
            profile.Map<TestObject, TestObjectDto>();

            var result = profile.Compile(compilationResult: true);

            Assert.NotNull(result);
            Assert.NotNull(result.Assembly);
            Assert.NotNull(result.Instance);
            Assert.NotNull(result.GeneratedCode);
        }

        [Fact]
        public void Compile_ReturnsGeneratedCode_WhenSuccessful()
        {
            var profile = new Profile();
            profile.Map<TestObject, TestObjectDto>();

            var sourceCode = profile.Compile(compilationResult: true);

            Assert.Contains("Mappee.UnitTests.Models.TestObjectDto __Mappee_UnitTests_Models_TestObject_to_Mappee_UnitTests_Models_TestObjectDto(Mappee.UnitTests.Models.TestObject source)", sourceCode.GeneratedCode);
            Assert.Contains(@"var classInstance = new Mappee.UnitTests.Models.TestObjectDto();", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Id = source.Id;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Bool = source.Bool;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Byte = source.Byte;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Char = source.Char;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.DateTime = source.DateTime;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Decimal = source.Decimal;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Email = source.Email;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.FirstName = source.FirstName;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Float = source.Float;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Int = source.Int;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.LastName = source.LastName;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Long = source.Long;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Nickname = source.Nickname;", sourceCode.GeneratedCode);
            Assert.Contains("classInstance.Short = source.Short;", sourceCode.GeneratedCode);
            Assert.Contains("return classInstance;", sourceCode.GeneratedCode);
        }

        [Fact]
        public void Compile_ReturnsNull_WhenCompilationResultIsFalse()
        {
            var profile = new Profile();
            profile.Map<TestObject, TestObjectDto>();

            var result = profile.Compile(compilationResult: false);

            Assert.Null(result);
        }

        [Fact]
        public void Compile_ReturnsGeneratedCodeWithoutOptimization_WhenAggressiveOptimizationOptionsDisabled()
        {
            var profile = new Profile();
            profile.Map<TestObject, TestObjectDto>();

            var result = profile.Compile(compilationResult: true, aggressiveOptimization: false);

            Assert.DoesNotContain("[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]", result.GeneratedCode);
        }

        [Fact]
        public void Compile_ReturnsGeneratedCodeWithOptimization_WhenAggressiveOptimizationOptionsEnabled()
        {
            var profile = new Profile();
            profile.Map<TestObject, TestObjectDto>();

            var result = profile.Compile(compilationResult: true, aggressiveOptimization: true);

            Assert.Contains("[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]", result.GeneratedCode);
        }
    }
}