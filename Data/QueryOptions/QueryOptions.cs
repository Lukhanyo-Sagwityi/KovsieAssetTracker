using System;
using System.Linq.Expressions;

namespace KovsieAssetTracker.Data.QueryOptions
{
    public class QueryOptions<T>
    {
        public Expression<Func<T, bool>>? Where { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = false;

        public bool HasWhere => Where != null;
        public bool HasPaging => PageNumber > 0 && PageSize > 0;
        public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);
        public int MaxPages { get; set; } = 3;
    }
}
