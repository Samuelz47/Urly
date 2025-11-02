using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urly.Application.DTOs;
public class ShortUrlDTO
{
    public string FullShortUrl { get; set; }
    public string LongURL { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreateAtUtc { get; set; }
}
