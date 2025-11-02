using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Application.DTOs;
using Urly.Domain.Entities;

namespace Urly.Application.Mappings;
public class ShortUrlMappingProfile : Profile
{
    public ShortUrlMappingProfile()
    {
        CreateMap<ShortUrlForRegistrationDTO, ShortUrl>();

        CreateMap<ShortUrl, ShortUrlDTO>();
        CreateMap<ShortUrl, UrlAnalyticsDTO>();
    }
}
