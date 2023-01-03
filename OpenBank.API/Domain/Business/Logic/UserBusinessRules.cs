using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OpenBank.API.Application.Interfaces;
using OpenBank.API.Domain.Business.Interfaces;
using OpenBank.API.Domain.Models.Entities;

namespace OpenBank.API.Domain.Business.Logic;

public class UserBusinessRules : IUserBusinessRules
{
    private readonly IUnitOfWork _unitOfwork;
    private readonly IMapper _mapper;

    public UserBusinessRules(IUnitOfWork unitOfwork, IMapper mapper)
    {
        _unitOfwork = unitOfwork;
        _mapper = mapper;
    }

    public async Task<User> CreateUserAsync(User createUserRequest)
    {
        try
        {
            PasswordHasher<string> pwHasher = new PasswordHasher<string>();

            createUserRequest.Password = pwHasher.HashPassword(createUserRequest.UserName, createUserRequest.Password);

            User createdUser = await _unitOfwork.userRepository.CreateUserAsync(createUserRequest);           

            return createdUser;
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

    public async Task<int> GetUserIdAsync(string username, string password)
    {
        try
        {
            User? existentUser = await _unitOfwork.userRepository.GetUserAsync(username);

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
        User? user = await _unitOfwork.userRepository.GetUserAsync(username);
        return user?.UserName.ToLower() != username.ToLower();
    }
}