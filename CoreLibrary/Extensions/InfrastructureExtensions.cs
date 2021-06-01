using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreLibrary.Common;
using CoreLibrary.CQRS;
using CoreLibrary.DDD;
using CoreLibrary.DDD.Entities;

namespace CoreLibrary.Extensions
{
    public static class InfrastructureExtensions
    {
        #region Dynamic Expression Compilation

        private static readonly ConcurrentDictionary<Expression, object> Cache
            = new();

        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this Expression<Func<TIn, TOut>> expr)
            //@see http://sergeyteplyakov.blogspot.ru/2015/06/lazy-trick-with-concurrentdictionary.html
            => (Func<TIn, TOut>)((Lazy<object>)Cache
                .GetOrAdd(expr, id => new Lazy<object>(expr.Compile))).Value;

        public static bool Is<T>(this T entity, Expression<Func<T, bool>> expr)
            => AsFunc(expr).Invoke(entity);

        public static Func<TIn, TOut> ToFunc<TIn, TOut>(this IQuery<TIn, TOut> query)
            => query.Ask;

        public static Func<TIn, TOut> ToFunc<TIn, TOut>(this ICommandHandler<TIn, TOut> commandHandler)
            => commandHandler.Handle;

        #endregion

        #region FP

        public static Func<TSource, TResult> Compose<TSource, TIntermediate, TResult>(
            this Func<TSource, TIntermediate> func1, Func<TIntermediate, TResult> func2)
            => x => func2(func1(x));

        public static TResult Forward<TSource, TResult>(
            this TSource source, Func<TSource, TResult> func)
            => func(source);

        public static T Match<T>(this T source
            , Func<T, bool> pattern
            , Func<T, T> evaluator)
            where T : class
            => pattern(source)
                ? evaluator(source)
                : source;

        public static T Match<T>(this object source, Func<object, T> evaluator)
            where T : class
            => Match(source as T, x => x != null, x => evaluator(x));

        public static TOutput If<TInput, TOutput>(this TInput o
            , Func<TInput, bool> condition
            , Func<TInput, TOutput> ifTrue
            , Func<TInput, TOutput> ifFalse)
            where TInput : class
            => condition(o) ? ifTrue(o) : ifFalse(o);


        public static TInput Do<TInput>(this TInput o, Action<TInput> action, Func<Exception> ifNull = null)
            where TInput : class
        {
            if (o == null)
            {
                if (ifNull != null)
                {
                    throw ifNull();
                }

                return null;
            }
            action(o);
            return o;
        }

        public static TOutput Do<TInput, TOutput>(this TInput o, Func<TInput, TOutput> func, Func<Exception> ifNull = null)
        {
            if (o == null)
            {
                if (ifNull != null)
                {
                    throw ifNull();
                }
            }

            return func(o);
        }

        #endregion

        #region Linq

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool cnd, Expression<Func<T, bool>> expr)
            => cnd
                ? queryable.Where(expr)
                : queryable;

        public static IQueryable<T> Apply<T>(this IQueryable<T> source, ILinqSpecification<T> spec)
            where T : class
            => spec.Apply(source);

        public static IQueryable<T> ApplyIfPossible<T>(this IQueryable<T> source, object spec)
            where T : class
            => spec is ILinqSpecification<T> specification
                ? specification.Apply(source)
                : source;

        public static IQueryable<TDest> Project<TSource, TDest>(this IQueryable<TSource> source, IProjector projector)
            => projector.Project<TSource, TDest>(source);

        public static TEntity ById<TEntity>(this ILinqProvider linqProvider, int id)
            where TEntity : class, IHasId<int>
            => linqProvider.GetQueryable<TEntity>().ById(id);

        public static TEntity ById<TEntity>(this IQueryable<TEntity> queryable, int id)
            where TEntity : class, IHasId<int>
            => queryable.SingleOrDefault(x => x.Id == id);

        #endregion

        #region Async

        public static Task<T> RunTask<T>(this Func<T> func)
            => Task.Run(func);


        public static TOut AskSync<TIn, TOut>(this IQuery<TIn, Task<TOut>> asyncQuery, TIn spec)
            => asyncQuery.Ask(spec).Result;

        #endregion

        #region Cqrs

        public static TResult Forward<TSource, TResult>(
            this TSource source, IQuery<TSource, TResult> query)
            => query.Ask(source);

        public static Task<TResult> Forward<TSource, TResult>(
            this TSource source, IQuery<TSource, Task<TResult>> query)
            => query.Ask(source);

        public static TResult Forward<TSource, TResult>(
            this TSource source, ICommandHandler<TSource, TResult> query)
            => query.Handle(source);

        public static void Forward<TSource>(
            this TSource source, ICommandHandler<TSource> query)
            => query.Handle(source);

        #endregion
    }
}
