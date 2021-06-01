using System;
using System.Linq.Expressions;
using CoreLibrary.Extensions;

namespace CoreLibrary.DDD.Specifications
{
    public class ExpressionSpecification<T> : ISpecification<T>
    {
        public virtual Expression<Func<T, bool>> Expression { get; private set; }

        private Func<T, bool> Func => Expression.AsFunc();

        public ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public bool IsSatisfiedBy(T o)
        {
            return Func(o);
        }
    } 
}
