using System;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.CQRS.Commands
{
    public class DeleteHandler<TKey, TEntity>
        : UowBased
        , ICommandHandler<TKey>
        where TEntity : class, IHasId<TKey>
    {
        public DeleteHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Handle(TKey key)
        {
            var entity = UnitOfWork.Find<TEntity>(key);
            if (entity == null)
            {
                throw new ArgumentException($"Entity {typeof(TEntity).Name} with id={key} doesn't exists");
            }

            UnitOfWork.Delete(entity);
            UnitOfWork.Commit();
        }

    }
}
