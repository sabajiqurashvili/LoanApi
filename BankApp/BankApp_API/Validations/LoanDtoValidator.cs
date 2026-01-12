using System.Data;
using BankApp_API.services.Models;
using BankApp_Domain;
using FluentValidation;

namespace BankApp_API.Validations;

public class LoanDtoValidator : AbstractValidator<LoanDto>
{
    public LoanDtoValidator()
    {
        RuleFor(dto => dto.LoanType).NotEmpty().WithMessage("LoanType is required");
        RuleFor(dto => dto.Amount).NotEmpty().WithMessage("Amount is required").GreaterThanOrEqualTo(500)
            .WithMessage("Amount must be greater than 500");
        RuleFor(dto => dto.LoanPeriodInMonths).NotEmpty().WithMessage("LoanPeriodInMonths is required").GreaterThanOrEqualTo(1).WithMessage("LoanPeriodInMonths must be greater than 1 month");
    }
        
}