using Mappee.Store.Models;
using Mappee.UnitTests.Models;

namespace Mappee.UnitTests.Store.Models;

public class TypeMappingCollectionTests
{
    [Fact]
    public void Add_AddsTypePairAndInstance()
    {
        var typePair = new TypeAssociation(typeof(object), typeof(object));
        var instance = new MethodExecutionPointer(new object(), IntPtr.Zero);

        TypeMappingCollection.Add(typePair, instance);
        var list = new TypeMappingCollection();

        Assert.Equal(instance, list[typeof(object), typeof(object)]!);
    }

    [Fact]
    public void Indexer_ReturnsCorrectInstance()
    {
        var testObjectToTestObjectDtoInstance = new MethodExecutionPointer(new object(), IntPtr.Zero);
        TypeMappingCollection.Add(new TypeAssociation(typeof(TestObject), typeof(TestObjectDto)), testObjectToTestObjectDtoInstance);

        var testObjectToObjectInstance = new MethodExecutionPointer(new object(), IntPtr.MinValue);
        TypeMappingCollection.Add(new TypeAssociation(typeof(TestObject), typeof(object)), testObjectToObjectInstance);

        var testObjectDtoToObjectInstance = new MethodExecutionPointer(new object(), IntPtr.MaxValue);
        TypeMappingCollection.Add(new TypeAssociation(typeof(TestObjectDto), typeof(object)), testObjectDtoToObjectInstance);

        var list = new TypeMappingCollection();

        Assert.Equal(testObjectToTestObjectDtoInstance, list[typeof(TestObject), typeof(TestObjectDto)]!);
        Assert.Equal(testObjectToObjectInstance, list[typeof(TestObject), typeof(object)]!);
        Assert.Equal(testObjectDtoToObjectInstance, list[typeof(TestObjectDto), typeof(object)]!);
    }

    [Fact]
    public void Indexer_ThrowsException_WhenTypePairNotFound()
    {
        TypeMappingCollection.Add(new TypeAssociation(typeof(int), typeof(string)), new MethodExecutionPointer(new object(), IntPtr.Zero));

        var list = new TypeMappingCollection();

        Assert.Throws<ArgumentOutOfRangeException>(() => list[typeof(string), typeof(int)]);
    }
}