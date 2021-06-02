using System;
using CoreLibrary.DDD;
using JetBrains.Annotations;

namespace CoreLibrary.CQRS
{
    public abstract class UowBased
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected UowBased([NotNull] IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
