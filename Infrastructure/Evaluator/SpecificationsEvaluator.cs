using Domain.Interfaces.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Evaluator
{
    public static class SpecificationsEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specifications)
        {
            var query = inputQuery;

            if (specifications.Criteria != null)
            {
                query = query.Where(specifications.Criteria);
            }

            if (specifications.OrderBy is not null)
            {
                query = query.OrderBy(specifications.OrderBy);
            }

            if (specifications.OrderByDescending is not null)
            {
                query = query.OrderByDescending(specifications.OrderByDescending);
            }

            if (specifications.IsPaginationEnabled)
            {
                query = query.Skip(specifications.Skip).Take(specifications.Take);
            }

            if (specifications.Includes.Any())
            {
                query = specifications.Includes
              .Aggregate(query, (current, include) => current.Include(include));
            }

            if (specifications.IncludeExpressions.Any())
            {
                query = specifications.IncludeExpressions
                    .Aggregate(query, (current, includeExpression) => includeExpression(current));
            }

            if (specifications.GroupByExpression != null)
            {
                query.GroupBy(specifications.GroupByExpression).SelectMany(x => x);
            }

            return query;
        }
    }
}
