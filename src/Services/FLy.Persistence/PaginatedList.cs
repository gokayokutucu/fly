using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace Fly.Persistence
{

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static async Task<PaginatedList<T>> CreateAsync(IMongoQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) return default;

            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}