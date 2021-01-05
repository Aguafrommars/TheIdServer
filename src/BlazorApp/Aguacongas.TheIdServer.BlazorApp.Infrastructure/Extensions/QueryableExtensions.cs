// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    /// <summary>
    /// Class d'extention de  <see cref="IQueryable{T}"/>
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Tri par
        /// </summary>
        /// <typeparam name="T">Le type à trier</typeparam>
        /// <param name="query">La requete à etendre</param>
        /// <param name="propertyName">Le nom de la prorpriété à trier</param>
        /// <param name="comparer">Un compareur</param>
        /// <returns>Une requete de tri</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        /// <summary>
        /// Tri descendant par
        /// </summary>
        /// <typeparam name="T">Le type à trier</typeparam>
        /// <param name="query">La requete à etendre</param>
        /// <param name="propertyName">Le nom de la prorpriété à trier</param>
        /// <param name="comparer">Un compareur</param>
        /// <returns>Une requete de tri</returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
                IComparer<object> comparer = null)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }

        /// <summary>
        /// Sort a query according to an order expression
        /// </summary>
        /// <typeparam name="TItem">The type of item</typeparam>
        /// <param name="query">The query to sort</param>
        /// <param name="orderByExpression">The order expresion</param>
        /// <param name="func">(Optional) a sort function, when not null this function is called to sort the query with the query, the property name and direction</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the sort function.</returns>
        public static IQueryable<TItem> Sort<TItem>(this IQueryable<TItem> query, string orderByExpression, Func<IQueryable<TItem>, string, string, IQueryable<TItem>> func = null)
        {
            orderByExpression = orderByExpression ?? throw new ArgumentNullException(nameof(orderByExpression));

            var terms = orderByExpression.Split(' ');
            var field = terms[0];
            var direction = terms.Length > 1 ? terms[1] : "ASC";
            if (func != null)
            {
                return func(query, field, direction);
            }

            if (direction.ToUpperInvariant() == "DESC")
            {
                return query.OrderByDescending(field);
            }

            return query.OrderBy(field);
        }
    }
}
