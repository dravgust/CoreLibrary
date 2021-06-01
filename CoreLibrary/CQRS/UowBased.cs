using System;
using CoreLibrary.DDD;

namespace CoreLibrary.CQRS
{
    public abstract class UowBased
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected UowBased(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
