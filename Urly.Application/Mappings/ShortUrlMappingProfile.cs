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

        CreateMap<ShortUrl, ShortUrlDTO>()
                .ForMember(dest => dest.FullShortUrl,
                            opt => opt.MapFrom(src => $"https://localhost:7032/{src.ShortCode}"));
    }
}
