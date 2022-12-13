using OpenBank.API.Models.Entities;

namespace OpenBank.API.Data;

public interface IUserRepository
{
    CreateUserRequest CreateUser(CreateUserRequest createUserRequest);
}

