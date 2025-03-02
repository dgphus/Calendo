using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendoApplication.Utils
{
    public interface IPaginatedList<TEntity>
    {
        IReadOnlyCollection<TEntity> Items { get; }
        int TotalItems { get; }
        int CurrentPage { get; }
        int TotalPages { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
