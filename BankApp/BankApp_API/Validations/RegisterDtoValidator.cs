using BankApp_API.services.Models;
using FluentValidation;

namespace BankApp_API.Validations;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(dto => dto.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(dto => dto.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(dto => dto.Age).GreaterThanOrEqualTo(18).WithMessage("Age must be greater than or equal 18");
        RuleFor(dto => dto.Salary).NotEmpty().WithMessage("Salary is required");
        RuleFor(dto  => dto.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(dto => dto.Password).NotEmpty().WithMessage("Password is required").MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}