using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OpenBank.API.Application.DTO;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.BusinessRules.Interfaces;
using OpenBank.API.Domain.Entities;

namespace OpenBank.API.BusinessRules;

public class UserBusinessRules : IUserBusinessRules
{
    private readonly IUnitOfWork _unitOfwork;
    private readonly IMapper _mapper;

    public UserBusinessRules(IUnitOfWork unitOfwork, IMapper mapper)
    {
        _unitOfwork = unitOfwork;
        _mapper = mapper;
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest)
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

            User created = await _unitOfwork.userRepository.CreateUserAsync(user);
            CreateUserResponse userDTO = _mapper.Map<User, CreateUserResponse>(created);

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
            List<User> users = _unitOfwork.userRepository.GetAllUsers();
            return users;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e.Message); //Log erro
            throw new Exception(e.Message);
        }
    }

    public async Task<int> GetUserIdAsync(LoginUserRequest login)
    {
        try
        {
            User? existentUser = await _unitOfwork.userRepository.GetUserAsync(login.UserName);

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

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        User? user = await _unitOfwork.userRepository.GetUserAsync(username);
        return user?.UserName.ToLower() != username.ToLower();
    }
}