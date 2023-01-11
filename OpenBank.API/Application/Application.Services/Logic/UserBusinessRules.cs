using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Repository.Interfaces;
using OpenBank.API.Application.Services.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Application.Services.Logic;

public class UserBusinessRules : IUserBusinessRules
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserBusinessRules(IUnitOfWork unitOfwork, IMapper mapper)
    {
        _unitOfWork = unitOfwork;
        _mapper = mapper;
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        try
        {
            PasswordHasher<string> pwHasher = new PasswordHasher<string>();

            User newUser = new User()
            {
                Email = createUserRequest.Email,
                FullName = createUserRequest.FullName,
                Password = pwHasher.HashPassword(createUserRequest.Username, createUserRequest.Password),
                UserName = createUserRequest.Username,
                Created_at = DateTime.UtcNow
            };

            var createdUser = await _unitOfWork.userRepository.CreateUserAsync(newUser);
            var userDTO = _mapper.Map<User, CreateUserResponse>(createdUser);

            return userDTO;
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
            var users = _unitOfWork.userRepository.GetAllUsers();
            return users;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    public async Task<int> GetUserIdAsync(string username, string password)
    {
        try
        {
            var existentUser = await _unitOfWork.userRepository.GetUserAsync(username);

            if (string.IsNullOrWhiteSpace(existentUser?.UserName))
                return 0;

            PasswordHasher<string> pwHasher = new PasswordHasher<string>();
            int validUser = (int)pwHasher.VerifyHashedPassword(
                                    username, existentUser?.Password, password
                                );

            return validUser > 0 ? existentUser.Id : 0;
        }
        catch (Exception e)
        {
            throw new Exception($"Server issues: {e.Message}");
        }
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        var user = await _unitOfWork.userRepository.GetUserAsync(username);
        return !string.Equals(user?.UserName, username, StringComparison.CurrentCultureIgnoreCase);
    }
}