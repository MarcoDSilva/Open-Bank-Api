using AutoMapper;
using OpenBank.API.Application.Repositories;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.DTO;

public class DtoMapper : Profile
{
    public DtoMapper()
    {
        CreateMap<Document, DocumentResponse>()
            .ForMember(target => target.Name, options => options
                .MapFrom(r => r.FileName));
        CreateMap<Account, AccountResponse>();
        CreateMap<Transfer, MovementResponse>().ReverseMap();
        CreateMap<User, CreateUserResponse>()
            .ForMember(target => target.PasswordChangedAt, options => options
                .MapFrom(r => r.Created_at));
    }
}