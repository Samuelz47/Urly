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
        bool exists = await _context.ShortUrls.AnyAsync(s => s.ShortCode == code);

        return !exists;
    }
}
