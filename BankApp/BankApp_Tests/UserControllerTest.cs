using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BankApp_API.Controllers;
using BankApp_API.services;
using BankApp_API.services.Models;
using BankApp_Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankApp_Tests;

public class UserControllerTest
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _userController;

    public UserControllerTest()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, "1")
            },
            "TestAuth"));

        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
    }


    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        //arrange
        _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(new User { Id = 1, FirstName = "Saba" });

        //act
        var result = await _userController.GetUserById(1);

        //assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<User>(okResult.Value);
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public async Task GetUserByIdAsync_NotFound_ShouldReturnNotFound()
    {
        //arrange
        _userServiceMock.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync((User)null);

        //act
        var result = await _userController.GetUserById(1);

        //assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RequestLoan_BlockedUser_ShouldReturnForbidden()
    {
        // Arrange
        var loanDto = new LoanDto
        {
            Amount = 1000,
            Currency = Currency.GEL,
            LoanPeriodInMonths = 12,
            LoanType = LoanType.QuickLoan
        };

        _userServiceMock
            .Setup(x => x.GetUserByIdAsync(1))
            .ReturnsAsync(new User
            {
                Id = 1,
                IsBlocked = true
            });

        // Act
        var result = await _userController.RequestLoan(loanDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetLoans_ShouldReturnOkWithLoans()
    {
        // Arrange
        var mockLoans = new List<Loan>
        {
            new Loan { Id = 1, Amount = 1000, UserId = 1, Status = Status.Proccesing },
            new Loan { Id = 2, Amount = 2000, UserId = 1, Status = Status.Proccesing }
        };

        _userServiceMock.Setup(x => x.GetLoans(1))
            .ReturnsAsync(mockLoans);
   
        // Act
        var result = await _userController.GetLoans();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedLoans = Assert.IsType<List<Loan>>(okResult.Value);
        Assert.Equal(2, returnedLoans.Count);
        Assert.All(returnedLoans, l => Assert.Equal(1, l.UserId));
    }
    [Fact]
    public async Task GetLoans_WhenNoLoans_ShouldReturnEmptyList()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetLoans(1))
            .ReturnsAsync(new List<Loan>());

        // Act
        var result = await _userController.GetLoans();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedLoans = Assert.IsType<List<Loan>>(okResult.Value);
        Assert.Empty(returnedLoans);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnOk_WhenLoanDeleted()
    {
        // Arrange
        var loanId = 1;
        _userServiceMock.Setup(x => x.DeleteLoan(1, loanId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userController.DeleteLoan(loanId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Loan has been deleted successfully", okResult.Value);
    }

    [Fact]
    public async Task DeleteLoan_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        var loanId = 1;
        _userServiceMock.Setup(x => x.DeleteLoan(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Loan not found"));

        // Act
        var result = await _userController.DeleteLoan(loanId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Loan not found", badRequest.Value);
    }
    [Fact]
    public async Task RegisterUser_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "saba",
            FirstName = "Saba",
            LastName = "Jiqurashvili",
            Age = 22,
            Salary = 5000,
            Password = "12345678"
        };

        var mockUser = new User { Id = 1, UserName = "saba" };

        _userServiceMock.Setup(s => s.RegisterUser(registerDto))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _userController.RegisterUser(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("saba", returnedUser.UserName);
    }
    
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "", 
            FirstName = "",
            LastName = "",
            Age = 0,
            Salary = 0
        };

        // Act
        var result = await _userController.RegisterUser(registerDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
        Assert.Contains(errors, e => e.Contains("UserName"));
    }
    
    
    [Fact]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "saba",
            FirstName = "Saba",
            LastName = "Jiqurashvili",
            Age = 22,
            Salary = 5000,
            Password = "12345678"
        };

        _userServiceMock.Setup(s => s.RegisterUser(registerDto))
            .ThrowsAsync(new Exception("Username saba is already taken"));

        // Act
        var result = await _userController.RegisterUser(registerDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Username saba is already taken", badRequest.Value);
    }
    [Fact]
    public async Task Login_ShouldReturnOk_WithToken_WhenValid()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "saba",
            Password = "12345678"
        };

        var loginResponse = new LoginResponse
        {
            Token = "FAKE.JWT.TOKEN",
            Role = Roles.User
        };

        _userServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(loginResponse);

        // Act
        var result = await _userController.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResponse = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal("FAKE.JWT.TOKEN", returnedResponse.Token);
        Assert.Equal(Roles.User, returnedResponse.Role);
    }
    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "",
            Password = ""
        };

        // Act
        var result = await _userController.Login(loginDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
        Assert.Contains(errors, e => e.Contains("Username") || e.Contains("Password"));
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenServiceThrows()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "saba",
            Password = "wrongpassword"
        };

        _userServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginDto>()))
            .ThrowsAsync(new Exception("Password wrongpassword not valid"));

        // Act
        var result = await _userController.Login(loginDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var obj = Assert.IsAssignableFrom<IDictionary<string, string>>(badRequest.Value);
        Assert.Equal("Password wrongpassword not valid", obj["message"]);
    }



}