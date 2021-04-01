using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog_API.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }
        public static IQueryable<T> IncludeIf<T>(this IQueryable<T> source,bool condition,Func<IQueryable<T>, IQueryable<T>> transform)
        {
            return condition ? transform(source) : source;
        }

        public static IQueryable<T> If<T, P>(
             this IIncludableQueryable<T, P> source,
             bool condition,
             Func<IIncludableQueryable<T, P>, IQueryable<T>> transform
         )
             where T : class
                {
                    return condition ? transform(source) : source;
                }

        public static IQueryable<T> If<T, P>(
            this IIncludableQueryable<T, IEnumerable<P>> source,
            bool condition,
            Func<IIncludableQueryable<T, IEnumerable<P>>, IQueryable<T>> transform
        )
            where T : class
            {
                return condition ? transform(source) : source;
            }
        }
}
