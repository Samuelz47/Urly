using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urly.Application.Interfaces;
using Urly.Domain.Repositories;

namespace Urly.Application.Services;
public class UrlClickService : IUrlClickService
{
    private readonly IUrlClickRepository _urlClickRepository;
    private readonly IUnitOfWork _uow;

    public UrlClickService(IUrlClickRepository urlClickRepository, IUnitOfWork uow)
    {
        _urlClickRepository = urlClickRepository;
        _uow = uow;
    }


}
