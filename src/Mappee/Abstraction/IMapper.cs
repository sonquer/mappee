namespace Mappee.Abstraction;

public interface IMapper
{
    TDestination Map<TSource, TDestination>(TSource source);

    TDestination Map<TDestination>(object source);
}