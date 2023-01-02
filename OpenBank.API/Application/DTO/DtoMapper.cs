using AutoMapper;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.Application.DTO;

public class DtoMapper : Profile
{
    public DtoMapper()
    {
        CreateMap<Account, AccountResponse>();
        CreateMap<Transfer, MovementResponse>().ReverseMap();
       

    }
}