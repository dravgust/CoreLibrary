using System;
using System.Linq;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;

namespace CorLibrary.Test.Stubs
{
    //public class TestDbContext : DbContext, ILinqProvider, IUnitOfWork
    //{
    //    public TestDbContext() : base("DefaultConnection")
    //    {
    //        Database.SetInitializer(new MigrateDatabaseToLatestVersion<TestDbContext, Migrations.Configuration>());
    //    }

    //    public IDbSet<Category> Categories { get; set; }

    //    public IDbSet<Product> Products { get; set; }
    //    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IHasId
    //    {
    //        return Set<TEntity>();
    //    }

    //    public IQueryable GetQueryable(Type t)
    //    {
    //        return Set(t);
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        //modelBuilder.Entity<Category>()
    //        //    .Property(s => s.Id).HasColumnName("Id").IsRequired();

    //        base.OnModelCreating(modelBuilder);
    //    }

    //    public void Add<TEntity>(TEntity entity) where TEntity : class, IHasId
    //    {
    //        base.Entry(entity).State = EntityState.Added;
    //    }

    //    public void Delete<TEntity>(TEntity entity) where TEntity : class, IHasId
    //    {
    //        Set<TEntity>().Delete(e => e.Id == entity.Id);
    //    }

    //    public TEntity Find<TEntity>(object id) where TEntity : class, IHasId
    //    {
    //        return Set<TEntity>().Find(id);
    //    }

    //    public IHasId Find(Type entityType, object id)
    //    {
    //        return Set(entityType).Find(id) as IHasId;
    //    }

    //    public void Commit()
    //    {
    //        SaveChanges();
    //    }
    //}
}
