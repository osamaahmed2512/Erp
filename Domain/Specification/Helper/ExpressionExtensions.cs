using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Specification.Helper
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
    this Expression<Func<T, bool>> expr1,
    Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;

            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> OrElse<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;

            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.OrElse(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
