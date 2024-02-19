using Mappee.Store.Models;

namespace Mappee.UnitTests.Store.Models;

public class TypeAssociationTests
{
    [Fact]
    public void Equals_ReturnsTrue_WhenSameInstance()
    {
        var typePair = new TypeAssociation(typeof(int), typeof(string));
        Assert.True(typePair.Equals(typePair));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenNull()
    {
        var typePair = new TypeAssociation(typeof(int), typeof(string));
        Assert.False(typePair.Equals(null));
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenSameTypes()
    {
        var typePair1 = new TypeAssociation(typeof(int), typeof(string));
        var typePair2 = new TypeAssociation(typeof(int), typeof(string));
        Assert.True(typePair1.Equals(typePair2));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenDifferentTypes()
    {
        var typePair1 = new TypeAssociation(typeof(int), typeof(string));
        var typePair2 = new TypeAssociation(typeof(string), typeof(int));
        Assert.False(typePair1.Equals(typePair2));
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenSameTypes()
    {
        var typePair1 = new TypeAssociation(typeof(int), typeof(string));
        var typePair2 = new TypeAssociation(typeof(int), typeof(string));
        Assert.Equal(typePair1.GetHashCode(), typePair2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_WhenDifferentTypes()
    {
        var typePair1 = new TypeAssociation(typeof(int), typeof(string));
        var typePair2 = new TypeAssociation(typeof(string), typeof(int));
        Assert.NotEqual(typePair1.GetHashCode(), typePair2.GetHashCode());
    }
}