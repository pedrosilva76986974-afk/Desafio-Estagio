using DESAFIO_ESTAGIO.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DESAFIO_ESTAGIO.Services;

public static class QueryExtensions
{
    public static async Task<ListResponse<T>> ToListResponseAsync<T>(this IQueryable<T> query, ListQuery request, CancellationToken ct)
    {
        var total = await query.CountAsync(ct);
        if (request.Page is int page)
        {
            var offset = ((long)page - 1) * request.PageSize;
            if (offset >= total) return new ListResponse<T>([], total, page, request.PageSize);
            query = query.Skip((int)offset).Take(request.PageSize);
        }
        return new ListResponse<T>(await query.ToListAsync(ct), total, request.Page, request.Page.HasValue ? request.PageSize : null);
    }
}
