using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urly.Application.DTOs;
public class ShortUrlForRegistrationDTO
{
    [Required]
    public string LongURL { get; set; }
}
