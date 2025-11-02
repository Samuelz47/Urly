using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Domain.Entities;
using Urly.Domain.Repositories;
using Urly.Infrastructure.Context;

namespace Urly.Infrastructure.Repositories;
public class UrlClickRepository : Repository<UrlClick>, IUrlClickRepository
{
    public UrlClickRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> CountByShortUrlIdAsync(int shortUrlId)
    {
        return await _context.UrlClickers.CountAsync(c => c.ShortUrlId == shortUrlId);
    }
}
