using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Expand<T>(this IQueryable<T> query, string expand) where T: class
        {
            if (expand != null)
            {
                var pathList = expand.Split(',');
                foreach (var path in pathList)
                {
                    query = query.Include(path.Trim().Replace('/', '.'));
                }
            }

            return query;
        }
    }
}
