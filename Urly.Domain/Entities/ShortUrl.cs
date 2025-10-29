using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urly.Domain.Entities;
public class ShortUrl
{
    public int Id { get; set; }
    [Required]
    public string LongURL { get; set; }
    [Required]
    public string ShortCode { get; set; }
    public DateTime CreateAtUtc { get; set; }
    public int? UserId { get; set; }
}
