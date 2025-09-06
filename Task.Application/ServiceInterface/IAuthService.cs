using System;
using Task.Application.DTO;
using Task.Domain.Entities;

namespace Task.Application.ServiceInterface;

public interface IAuthService
{
    User RegisterUser(RegisterUserDto registerDto);
    User? LoginUser(LoginUserDto loginDto);
    bool IsUsernameAvailable(string username);
    bool IsEmailAvailable(string email);

}
