using System;
using CoreLibrary.Common;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.CQRS.Commands
{
    public class CreateOrUpdateHandler<TKey, TDto, TEntity> : UowBased, ICommandHandler<TDto, TKey>
        where TKey: struct
        where TEntity : HasIdBase<TKey>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateOrUpdateHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public TKey Handle(TDto context)
        {
            var id = (context as IHasId)?.Id;
            var entity = id != null && !default(TKey).Equals(id)
                ? _mapper.Map(context, _unitOfWork.Find<TEntity>(id))
                : _mapper.Map<TEntity>(context);

            if (entity.IsNew())
            {
                UnitOfWork.Add(entity);
            }

            UnitOfWork.Commit();
            return entity.Id;
        }
    }
}
