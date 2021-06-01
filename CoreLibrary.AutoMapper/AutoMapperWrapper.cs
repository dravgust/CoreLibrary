using System.Linq;
using AutoMapper.QueryableExtensions;
using CoreLibrary.Common;
using AM = AutoMapper;
using IMapper = CoreLibrary.Common.IMapper;

namespace CoreLibrary.AutoMapper
{
    public class AutoMapperWrapper : IMapper, IProjector
    {
        public AM.IConfigurationProvider Configuration { get; private set; }
        public AM.IMapper Instance { get; private set; }

        public AutoMapperWrapper(AM.IConfigurationProvider configuration, bool skipValidnessAssert = false)
        {
            Configuration = configuration;

            if (!skipValidnessAssert)
            {
                configuration.AssertConfigurationIsValid();
            }

            Instance = configuration.CreateMapper();
        }

        public TReturn Map<TReturn>(object src) => Instance.Map<TReturn>(src);

        public TReturn Map<TReturn>(object src, TReturn dest) => Instance.Map(src, dest);

        public IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable)
            => queryable.ProjectTo<TReturn>(Configuration);
    }
}
