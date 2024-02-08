namespace Mappe
{
    public class MappeExecutor : IMapper
    {
        public T2 Map<T1, T2>(T1 source)
        {
            return Mapper.Map<T1, T2>(source);
        }
    }
}
