using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return source.OrderBy(ToLambda<T>(property));
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return source.OrderByDescending(ToLambda<T>(property));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string property)
        {
            var parameter = Expression.Parameter(typeof(T));
            var propertyExpression = Expression.Property(parameter, property);
            return Expression.Lambda<Func<T, object>>(Expression.Convert(propertyExpression, typeof(object)), parameter);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var selector = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, selector });
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var selector = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, selector });
        }
    }

}
