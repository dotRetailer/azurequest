using AzureQuest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AzureQuest.Api.Extensions
{
    public static class QuerySortExtensions
    {
        public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> query, IEnumerable<DataOrderItem> orders)
        {
            if (orders != null && orders.Any())
            {
                foreach (var order in orders)
                {
                    query = query.OrderBy(order.Property, order.Asc);
                }
            }
            return query;
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, IEnumerable<DataOrderItem> orders)
        {
            if (orders != null && orders.Any())
            {
                foreach (var order in orders)
                {
                    query = query.OrderBy(order.Property, order.Asc);
                }
            }
            return query;
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string key, bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return query;
            }

            var lambda = (dynamic)CreateExpression(typeof(TSource), key);

            return ascending
                ? Queryable.OrderBy(query, lambda)
                : Queryable.OrderByDescending(query, lambda);
        }

        public static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> query, string key, bool ascending = true)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return query;
            }

            var lambda = (dynamic)CreateExpression(typeof(TSource), key);

            return ascending
                ? Queryable.OrderBy(query, lambda)
                : Queryable.OrderByDescending(query, lambda);
        }

        private static LambdaExpression CreateExpression(Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");

            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }

            return Expression.Lambda(body, param);
        }
    }
}
