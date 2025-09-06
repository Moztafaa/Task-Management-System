using System;
using System.Net;
using Task.Application.DTO;
using Task.Application.ServiceInterface;
using Task.Application.Services;
using Task.Application.Utilities;
using Task.Domain.Entities;
using Task.Domain.RepositoryInterface;

namespace Task.Application.ServiceImplementation;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    public User RegisterUser(RegisterUserDto registerDto)
    {
        ValidateRegistration(registerDto);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = PasswordHelper.HashPassword(registerDto.Password!),
            CreatedAt = DateTime.UtcNow
        };

        userRepository.Add(user);
        return user;

    }

    public User? LoginUser(LoginUserDto loginDto)
    {
        var user = userRepository.GetByUsername(loginDto.Username!);
        if (user is not null && PasswordHelper.VerifyPassword(loginDto.Password!, user.PasswordHash!))
        {
            user.LastLoginAt = DateTime.UtcNow;
            userRepository.Update(user);

            // Set session
            SessionManager.Instance.Login(user);

            return user;
        }
        return null;

    }

    public bool IsEmailAvailable(string email)
    {
        return !userRepository.ExistsByEmail(email);
    }

    public bool IsUsernameAvailable(string username)
    {
        return !userRepository.ExistsByUsername(username);
    }


    private void ValidateRegistration(RegisterUserDto registerDto)
    {
        if (!IsUsernameAvailable(registerDto.Username!))
            throw new ArgumentException("Username is already taken");

        if (!IsEmailAvailable(registerDto.Email!))
            throw new ArgumentException("Email is already registered");

        if (registerDto.Password != registerDto.ConfirmPassword)
            throw new ArgumentException("Passwords do not match");

    }
}
