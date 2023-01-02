using Microsoft.AspNetCore.Identity;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessRules.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules;

public class UserBusinessRules : IUserBusinessRules
{
    private readonly IUnitOfWork _unitOfwork;

    public UserBusinessRules(IUnitOfWork unitOfwork)
    {
        _unitOfwork = unitOfwork;
    }

    public async Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest)
    {
        try
        {
            PasswordHasher<string> pwHasher = new PasswordHasher<string>();

            User user = new User()
            {
                Email = createUserRequest.Email,
                FullName = createUserRequest.FullName,
                Password = pwHasher.HashPassword(createUserRequest.Username, createUserRequest.Password),
                UserName = createUserRequest.Username,
                Created_at = DateTime.UtcNow
            };

            CreateUserResponse created = await _unitOfwork.userRepository.CreateUser(user);

            return created;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    public List<User> GetAllUsers()
    {
        try
        {
            List<User> users = _unitOfwork.userRepository.GetAllUsers();
            return users;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    public async Task<int> GetUserId(LoginUserRequest login)
    {
        try
        {
            User? existentUser = await _unitOfwork.userRepository.GetUser(login.UserName);

            if (string.IsNullOrWhiteSpace(existentUser?.UserName))
                return 0;

            PasswordHasher<string> pwHasher = new PasswordHasher<string>();
            int validUser = (int)pwHasher.VerifyHashedPassword(
                                    login.UserName, existentUser?.Password, login.Password
                                );

            return validUser > 0 ? existentUser.Id : 0;
        }
        catch (Exception e)
        {
            throw new Exception($"Server issues: {e.Message}");
        }
    }

    public async Task<bool> IsUsernameAvailable(string username)
    {
        User? user = await _unitOfwork.userRepository.GetUser(username);
        return user?.UserName.ToLower() != username.ToLower();
    }
}