using BankApp_API.services.Models;
using FluentValidation;

namespace BankApp_API.Validations;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(loginDto => loginDto.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(loginDto => loginDto.Password).NotEmpty().WithMessage("Password is required").MinimumLength(8).WithMessage("password must be at least 8 characters long");
    }
}