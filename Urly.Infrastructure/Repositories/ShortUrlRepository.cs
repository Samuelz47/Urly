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
public class ShortUrlRepository : Repository<ShortUrl>, IShortUrlRepository
{
    public ShortUrlRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> IsCodeUniqueAsync(string code)
    {
        var shortUrl = await _context.ShortUrls
                                     .FirstOrDefaultAsync(s => s.ShortCode == code);

        if (shortUrl is null) { return false; }

        return true;
    }
}
