using System;
using AutoMapper;
using CoreLibrary.AutoMapper;
using CoreLibrary.Common;
using CoreLibrary.Components;
using CoreLibrary.DDD;
using CorLibrary.Test.Stubs;
using Moq;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace CorLibrary.Test.AutoMapper
{
    public class DtoToEntityConverterTest
    {
        public readonly Container Container;

        public DtoToEntityConverterTest()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
        }


        [SetUp]
        public void Init()
        {
            var currentAssembly = typeof(DtoToEntityConverterTest).Assembly;
            var componentMap = AutoRegistration.GetComponentMap(currentAssembly,
                x => typeof(ProductController).IsAssignableFrom(x),
                currentAssembly, x => x.IsInterface);

            var am = Lifestyle.Singleton.CreateRegistration(() =>
            {
                ConventionalProfile.Scan(currentAssembly);
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.ConstructServicesUsing(Container.GetInstance);
                    cfg.AddProfile<ConventionalProfile>();
                    //cfg.CreateProfile("DtoMapperProfile", prf =>
                    //{
                    //    //prf.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                    //    //prf.DestinationMemberNamingConvention = new PascalCaseNamingConvention();;
                    //    prf.CreateMap<CreateProductDto, Product>()
                    //        .ForMember(dest => dest.Id, opt => opt.Ignore())
                    //        .ForMember(dest => dest.Category, opt => opt.MapFrom<ProductResolver>());
                    //});
                }
                );
                return new AutoMapperWrapper(configuration);
            }, Container);

            Container.AddRegistration(typeof(IProjector), am);
            Container.AddRegistration(typeof(CoreLibrary.Common.IMapper), am);

            var reg = Lifestyle.Singleton.CreateRegistration(() => new FakeContext(), Container);

            Container.AddRegistration(typeof(ILinqProvider), reg);
            Container.AddRegistration(typeof(IUnitOfWork), reg);

            foreach (var kv in componentMap)
            {
                Container.Register(kv.Key, kv.Value, Lifestyle.Scoped);
            }

            Container.Register<DtoEntityTypeConverter<int, CreateProductDto, Product>>(Lifestyle.Transient);
            //container.Register(typeof(DtoEntityTypeConverter<,,>), currentAssembly, Lifestyle.Scoped);

            Container.Verify();
        }

        [Test]
        public void Convert_Success()
        {
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x
                    .Find<Category>(It.IsAny<object>()))
                .Returns(new Category(10, "Super") { Id = 1 });

            var converter = new DtoEntityTypeConverter<int, ProductDto, Product>(uow.Object);
            var res = converter.Convert(new ProductDto() { CategoryId = 1, Price = 100500 }, null, null);

            Assert.AreEqual(100500, res.Price);
        }

        [Test]
        public void DtoToEntityMap_Success()
        {
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x
                    .Find<Category>(It.IsAny<object>()))
                .Returns(new Category(10, "Super") { Id = 1 });

            var mapper = Container.GetInstance<CoreLibrary.Common.IMapper>();
            var linqProvider = Container.GetInstance<ILinqProvider>();
            var unitOfWork = Container.GetInstance<IUnitOfWork>();

            var res = mapper.Map<Product>(new CreateProductDto { CategoryId = 1, Price = 100500 });

            Assert.AreEqual(100500, res.Price);
        }
    }
}
