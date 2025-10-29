using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urly.Domain.Entities;
public class UrlClick
{
    public int Id { get; set; }
    public int ShortUrlId { get; set; }
    public DateTime ClickedAtUtc { get; set; }
}
