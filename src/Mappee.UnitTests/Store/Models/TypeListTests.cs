using Mappee.Store.Models;
using Mappee.UnitTests.Models;

namespace Mappee.UnitTests.Store.Models;

public class TypeListTests
{
    [Fact]
    public void Add_AddsTypePairAndInstance()
    {
        var typePair = new TypePair(typeof(object), typeof(object));
        var instance = new InstancePointer(new object(), IntPtr.Zero);

        TypeList.Add(typePair, instance);
        var list = new TypeList();

        Assert.Equal(instance, list[typeof(object), typeof(object)]!);
    }

    [Fact]
    public void Indexer_ReturnsCorrectInstance()
    {
        var testObjectToTestObjectDtoInstance = new InstancePointer(new object(), IntPtr.Zero);
        TypeList.Add(new TypePair(typeof(TestObject), typeof(TestObjectDto)), testObjectToTestObjectDtoInstance);

        var testObjectToObjectInstance = new InstancePointer(new object(), IntPtr.MinValue);
        TypeList.Add(new TypePair(typeof(TestObject), typeof(object)), testObjectToObjectInstance);

        var testObjectDtoToObjectInstance = new InstancePointer(new object(), IntPtr.MaxValue);
        TypeList.Add(new TypePair(typeof(TestObjectDto), typeof(object)), testObjectDtoToObjectInstance);

        var list = new TypeList();

        Assert.Equal(testObjectToTestObjectDtoInstance, list[typeof(TestObject), typeof(TestObjectDto)]!);
        Assert.Equal(testObjectToObjectInstance, list[typeof(TestObject), typeof(object)]!);
        Assert.Equal(testObjectDtoToObjectInstance, list[typeof(TestObjectDto), typeof(object)]!);
    }

    [Fact]
    public void Indexer_ThrowsException_WhenTypePairNotFound()
    {
        TypeList.Add(new TypePair(typeof(int), typeof(string)), new InstancePointer(new object(), IntPtr.Zero));

        var list = new TypeList();

        Assert.Throws<ArgumentOutOfRangeException>(() => list[typeof(string), typeof(int)]);
    }
}