using System.Collections.Generic;
using System.Linq;

namespace SlingleBlog.Common.Paging
{
    public static class OrderedQueryableExtensions
    {
        public static List<TEntity> ToList<TEntity>(this IOrderedQueryable<TEntity> query, PagingData pagingData)
        {
            var pageSize = pagingData.PageSize + 1;
            pagingData.HasPreviousPage = pagingData.Page > 1;

            var result = query.Skip((pagingData.Page - 1) * pagingData.PageSize).Take(pageSize).ToList();
            if (pageSize == result.Count())
            {
                pagingData.HasNextPage = true;
                return result.Take(pagingData.PageSize).ToList();
            }
            pagingData.HasNextPage = false;
            return result;
        }

    }
}
