using System.Security.Claims;
using BankApp_API.Controllers;
using BankApp_API.services;
using BankApp_API.services.Models;
using BankApp_Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankApp_Tests;

public class AccountantControllerTest
{
    private readonly Mock<IAccountantService> _accountantServiceMock;
    private readonly AccountantController _controller;

    public AccountantControllerTest()
    {
        _accountantServiceMock = new Mock<IAccountantService>();
        _controller = new AccountantController(_accountantServiceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "1") }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
    
    
    [Fact]
    public async Task SeeLoans_ShouldReturnBadRequest_WhenUserIdInvalid()
    {
        // Arrange
        int invalidUserId = -1;

        // Act
        var result = await _controller.SeeLoans(invalidUserId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid userId", badRequest.Value);
    }
    
    [Fact]
    public async Task SeeLoans_ShouldReturnNotFound_WhenNoLoans()
    {
        // Arrange
        int userId = 1;
        _accountantServiceMock.Setup(s => s.SeeLoans(userId))
            .ReturnsAsync(new List<Loan>());

        // Act
        var result = await _controller.SeeLoans(userId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No loans found", notFound.Value);
    }
    
    [Fact]
    public async Task SeeLoans_ShouldReturnOk_WithLoans()
    {
        // Arrange
        int userId = 1;
        var loans = new List<Loan>
        {
            new Loan { Id = 1, Amount = 1000, UserId = userId, Status = Status.Proccesing },
            new Loan { Id = 2, Amount = 2000, UserId = userId, Status = Status.Proccesing }
        };
        _accountantServiceMock.Setup(s => s.SeeLoans(userId)).ReturnsAsync(loans);

        // Act
        var result = await _controller.SeeLoans(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedLoans = Assert.IsType<List<Loan>>(okResult.Value);
        Assert.Equal(2, returnedLoans.Count);
        Assert.All(returnedLoans, l => Assert.Equal(userId, l.UserId));
    }
    
    [Fact]
    public async Task SeeLoans_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        int userId = 1;
        _accountantServiceMock.Setup(s => s.SeeLoans(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.SeeLoans(userId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Database error", badRequest.Value);
    }
    
    [Fact]
    public async Task UpdateLoan_ShouldReturnBadRequest_WhenUserIdOrLoanIdInvalid()
    {
        // Arrange
        int invalidUserId = -1;
        int invalidLoanId = -1;
        var loanDto = new LoanDtoForAccountant { Status = Status.Approved };

        // Act
        var result = await _controller.UpdateLoan(invalidUserId, invalidLoanId, loanDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("userId and loanId cant be null", badRequest.Value);
    }

    [Fact]
    public async Task UpdateLoan_ShouldReturnOk_WhenServiceSucceeds()
    {
        //Arrange
        int userId = 1;
        int loanId = 1;
        var loanDto = new LoanDtoForAccountant { Status = Status.Approved };
        _accountantServiceMock.Setup(s => s.UpdateLoan(userId, loanId, loanDto)).ReturnsAsync(new Loan(){UserId = userId,Id = loanId,Status = Status.Approved});
        
        //Act
        var result = await _controller.UpdateLoan(userId, loanId, loanDto);
        
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"loan status changed to {loanDto.Status}", okResult.Value);
        
    }
    
    
    [Fact]
    public async Task UpdateLoan_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        int userId = 1;
        int loanId = 1;
        var loanDto = new LoanDtoForAccountant { Status = Status.Approved };

        _accountantServiceMock.Setup(s => s.UpdateLoan(userId, loanId, loanDto))
            .ThrowsAsync(new Exception("Loan not found"));

        // Act
        var result = await _controller.UpdateLoan(userId, loanId, loanDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Loan not found", badRequest.Value);
    }
    
    [Fact]
    public async Task DeleteLoan_ShouldReturnBadRequest_WhenUserIdOrLoanIdInvalid()
    {
        // Arrange
        int userId = -1;
        int loanId = -1;

        // Act
        var result = await _controller.DeleteLoan(userId, loanId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("userId and loanId cant be less than 0", badRequest.Value);
    }
    [Fact]
    public async Task DeleteLoan_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        int userId = 1;
        int loanId = 1;

        _accountantServiceMock.Setup(s => s.DeleteLoan(userId, loanId))
            .ThrowsAsync(new Exception("Loan not found"));

        // Act
        var result = await _controller.DeleteLoan(userId, loanId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Loan not found", badRequest.Value);
    }
    
    [Fact]
    public async Task BlockUser_ShouldReturnBadRequest_WhenUserIdInvalid()
    {
        // Arrange
        int invalidUserId = -1;

        // Act
        var result = await _controller.BlockUser(invalidUserId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid userId", badRequest.Value);
    }

    [Fact]
    public async Task BlockUser_ShouldReturnOk_WhenServiceSucceeds()
    {
        // Arrange
        int userId = 1;

        _accountantServiceMock.Setup(s => s.BlockUser(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.BlockUser(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal($"UserID : {userId} has been blocked", okResult.Value);
    }

    [Fact]
    public async Task BlockUser_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        int userId = 1;

        _accountantServiceMock.Setup(s => s.BlockUser(userId))
            .ThrowsAsync(new Exception("User not found"));

        // Act
        var result = await _controller.BlockUser(userId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User not found", badRequest.Value);
    }


    
    
    
}
