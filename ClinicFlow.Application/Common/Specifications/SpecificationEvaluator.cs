namespace ClinicFlow.Application.Common.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, BaseSpecification<T> spec) where T:class
        {
            var query = inputQuery;

            foreach(var criteria in spec.Criteria)
            {
                query = query.Where(criteria);
            }

            if(spec.OrderBy!=null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            return query;


        }


    }
}
