using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Domain.Entities;

namespace Urly.Domain.Repositories;
public interface IShortUrlRepository : IRepository<ShortUrl>
{
    Task<bool> IsCodeUniqueAsync(string code);
}
