namespace CoreLibrary.Common
{
    public interface IMapper
    {
        TReturn Map<TReturn>(object src);

        TReturn Map<TReturn>(object src, TReturn dest);
    }
}