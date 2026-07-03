using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GrowStore.Application.Auth.DTOs;
using GrowStore.Application.Auth.Interfaces;
using GrowStore.Application.Auth.Services;
using GrowStore.Application.Auth.Settings;
using GrowStore.Application.Common.Errors;
using GrowStore.Domain.Entities;
using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Interfaces;
using GrowStore.Domain.Shared.Enums;
using Microsoft.Extensions.Options;
using Moq;

namespace Growstore.Tests.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IAccountRepository> _accountRepoMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IValidator<RegisterRequestDto>> _registerValidatorMock;
    private readonly Mock<IValidator<LoginRequestDto>> _loginValidatorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _accountRepoMock = new Mock<IAccountRepository>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _registerValidatorMock = new Mock<IValidator<RegisterRequestDto>>();
        _loginValidatorMock = new Mock<IValidator<LoginRequestDto>>();

        var jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "test-secret-key-1234567890",
            RefreshTokenExpiryDays = 7,
            Issuer = "test",
            Audience = "test",
            ExpiryMinutes = 60
        });

        _registerValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<RegisterRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tokenServiceMock
            .Setup(t => t.GenerateToken(It.IsAny<TokenRequestDto>()))
            .ReturnsAsync("fake-access-token");

        _tokenServiceMock
            .Setup(t => t.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        _authService = new AuthService(
            _userRepoMock.Object,
            _accountRepoMock.Object,
            _refreshTokenRepoMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            jwtSettings,
            _registerValidatorMock.Object,
            _loginValidatorMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidDto_ReturnsValidationError()
    {
        var dto = new RegisterRequestDto();
        var errors = new List<ValidationFailure> { new("Email", "Email is required.") };

        _registerValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _authService.RegisterAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Auth.Validation");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsConflictError()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Test",
            Cpf = "12345678909",
            BirthDate = new DateTime(1990, 1, 1),
            Email = "test@test.com",
            Password = "Password123!"
        };

        _accountRepoMock
            .Setup(r => r.EmailExistsAsync(dto.Email))
            .ReturnsAsync(true);

        var result = await _authService.RegisterAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error!.Code.Should().Be("Account.EmailAlreadyExists");
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsSuccess()
    {
        var dto = new RegisterRequestDto
        {
            Name = "Test User",
            Cpf = "12345678909",
            BirthDate = new DateTime(1990, 1, 1),
            Email = "test@test.com",
            Password = "Password123!"
        };

        _accountRepoMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _passwordHasherMock.Setup(p => p.Hash(dto.Password)).Returns("hashed-password");

        var result = await _authService.RegisterAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("fake-access-token");
        result.Value!.RefreshToken.Should().Be("fake-refresh-token");
        result.Value!.Email.Should().Be(dto.Email);
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _accountRepoMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidDto_ReturnsValidationError()
    {
        var dto = new LoginRequestDto();
        var errors = new List<ValidationFailure> { new("Email", "Email is required.") };

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        var result = await _authService.LoginAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Auth.Validation");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "wrong" };

        _accountRepoMock
            .Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync((Account?)null);

        var result = await _authService.LoginAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
        result.Error!.Code.Should().Be("Auth.InvalidCredentials");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsUnauthorized()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "wrong" };
        var account = new Account(Guid.NewGuid(), dto.Email, "hashed");

        _accountRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(account);
        _passwordHasherMock.Setup(p => p.Verify(dto.Password, account.Password)).Returns(false);

        var result = await _authService.LoginAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var user = User.Create("Test", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);
        var account = new Account(user.Id, dto.Email, "hashed");

        _accountRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(account);
        _passwordHasherMock.Setup(p => p.Verify(dto.Password, account.Password)).Returns(true);
        _userRepoMock.Setup(r => r.GetUserByIdAsync(account.UserId)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be(dto.Email);
        result.Value!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsNotFound()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Password123!" };
        var account = new Account(Guid.NewGuid(), dto.Email, "hashed");

        _accountRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(account);
        _passwordHasherMock.Setup(p => p.Verify(dto.Password, account.Password)).Returns(true);
        _userRepoMock.Setup(r => r.GetUserByIdAsync(account.UserId)).ReturnsAsync((User?)null);

        var result = await _authService.LoginAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task RefreshAsync_WithInvalidToken_ReturnsUnauthorized()
    {
        var dto = new RefreshTokenRequestDto { RefreshToken = "invalid-token" };

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync(dto.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        var result = await _authService.RefreshAsync(dto);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task RefreshAsync_WithValidToken_ReturnsNewTokens()
    {
        var user = User.Create("Test", "123.456.789-09", new DateTime(1990, 1, 1), UserRole.CUSTOMER);
        var account = new Account(user.Id, "test@test.com", "hashed");
        var storedToken = new RefreshToken(account.Id, "old-refresh-token", DateTime.UtcNow.AddDays(7));

        var dto = new RefreshTokenRequestDto { RefreshToken = "old-refresh-token" };

        _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync(dto.RefreshToken)).ReturnsAsync(storedToken);
        _accountRepoMock.Setup(r => r.GetByIdAsync(storedToken.AccountId)).ReturnsAsync(account);
        _userRepoMock.Setup(r => r.GetUserByIdAsync(account.UserId)).ReturnsAsync(user);

        var result = await _authService.RefreshAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("fake-access-token");
        result.Value!.RefreshToken.Should().Be("fake-refresh-token");
        _refreshTokenRepoMock.Verify(r => r.UpdateAsync(storedToken), Times.Once);
    }
}
