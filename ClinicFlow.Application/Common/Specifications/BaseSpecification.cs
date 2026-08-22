using System.Linq.Expressions;

namespace ClinicFlow.Application.Common.Specifications
{
    public abstract class BaseSpecification<T>
    {

        public List<Expression<Func<T, bool>>> Criteria { get; } = new();

        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        protected void AddCriteria(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria.Add(criteriaExpression);
        }

        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }


    }
}
